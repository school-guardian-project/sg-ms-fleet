using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Application.Dtos;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class ListBusesBasicService : IListBusesBasicUseCase
{
    private readonly IBusRepository _busRepository;
    private readonly IDriverAssignmentRepository _assignmentRepository;
    private readonly IIamService _iamService;
    private readonly ITenantProvider _tenantProvider;
    private readonly ICampusReferenceRepository _campusReferenceRepository;

    public ListBusesBasicService(
        IBusRepository busRepository,
        IDriverAssignmentRepository assignmentRepository,
        IIamService iamService,
        ITenantProvider tenantProvider,
        ICampusReferenceRepository campusReferenceRepository)
    {
        _busRepository = busRepository;
        _assignmentRepository = assignmentRepository;
        _iamService = iamService;
        _tenantProvider = tenantProvider;
        _campusReferenceRepository = campusReferenceRepository;
    }

    public async Task<IReadOnlyList<BusListItemDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var campusFilter = await ResolveCampusFilterAsync(ct);
        var buses = await _busRepository.GetAllAsync(campusFilter, ct);
        var result = new List<BusListItemDto>();
        var modelNames = new Dictionary<byte, ModelNames?>();

        foreach (var bus in buses)
        {
            var assignment = await _assignmentRepository.GetActiveByBusIdAsync(bus.Id, ct);
            string driverName = string.Empty;

            if (assignment != null)
            {
                driverName = await _iamService.GetDriverNameAsync(assignment.ProfileId, ct) ?? string.Empty;
            }

            if (!modelNames.TryGetValue(bus.ModelId, out var names))
            {
                names = await _busRepository.GetModelNamesAsync(bus.ModelId, ct);
                modelNames[bus.ModelId] = names;
            }

            result.Add(new BusListItemDto(
                bus.Id,
                bus.Plate,
                bus.CampuseId,
                driverName,
                names?.Brand ?? string.Empty,
                names?.Model ?? string.Empty));
        }

        return result;
    }

    /// <summary>
    /// Sedes permitidas para el tenant actual; null = sin filtro (anónimo, llamada interna o SuperAdmin).
    /// Admin (roleId 1): todas las sedes de su colegio. Student/Driver/Parent (2/3/4): solo su sede.
    /// </summary>
    private async Task<IReadOnlyCollection<Guid>?> ResolveCampusFilterAsync(CancellationToken ct)
    {
        if (!_tenantProvider.ShouldFilter) return null;

        if (_tenantProvider.RoleId == 1 && _tenantProvider.SchoolId is { } schoolId)
            return await _campusReferenceRepository.GetCampusIdsBySchoolAsync(schoolId, ct);

        if (_tenantProvider.CampusId is { } campusId)
            return [campusId];

        return null;
    }
}
