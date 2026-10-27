using ms_fleet.Api.Application.Security;
using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class GetBusAssignedToDriverService : IGetBusAssignedToDriverUseCase
{
    private readonly IDriverAssignmentRepository _assignmentRepository;
    private readonly IBusRepository _busRepository;
    private readonly IBusMapper _busMapper;
    private readonly BusAccessPolicy _accessPolicy;

    public GetBusAssignedToDriverService(
        IDriverAssignmentRepository assignmentRepository,
        IBusRepository busRepository,
        IBusMapper busMapper,
        BusAccessPolicy accessPolicy)
    {
        _assignmentRepository = assignmentRepository;
        _busRepository = busRepository;
        _busMapper = busMapper;
        _accessPolicy = accessPolicy;
    }

    /// <exception cref="BusAccessDeniedException">
    /// El perfil autenticado no es ese conductor ni personal con buses.read.
    /// </exception>
    public async Task<BusAssignedResponse?> ExecuteAsync(Guid profileId, CancellationToken ct = default)
    {
        _accessPolicy.EnsureCanReadDriverBus(profileId);

        var assignment = await _assignmentRepository.GetActiveByProfileIdAsync(profileId, ct);
        if (assignment is null) return null;

        var bus = await _busRepository.GetByIdAsync(assignment.BusId, ct);
        if (bus is null || bus.Status != Status.Active) return null;

        // El personal solo ve los buses de su colegio; el propio conductor siempre ve el suyo.
        if (!_accessPolicy.IsSelf(profileId) && !await _accessPolicy.CanSeeBusAsync(bus, ct)) return null;

        return _busMapper.ToAssigned(bus);
    }
}