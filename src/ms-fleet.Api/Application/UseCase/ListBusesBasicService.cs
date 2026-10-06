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

    public ListBusesBasicService(
        IBusRepository busRepository,
        IDriverAssignmentRepository assignmentRepository,
        IIamService iamService)
    {
        _busRepository = busRepository;
        _assignmentRepository = assignmentRepository;
        _iamService = iamService;
    }

    public async Task<IReadOnlyList<BusListItemDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var buses = await _busRepository.GetAllAsync(ct);
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
}
