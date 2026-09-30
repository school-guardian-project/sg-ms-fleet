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

        foreach (var bus in buses)
        {
            var assignment = await _assignmentRepository.GetActiveByBusIdAsync(bus.Id, ct);
            string driverName = string.Empty;

            if (assignment != null)
            {
                driverName = await _iamService.GetDriverNameAsync(assignment.ProfileId, ct) ?? string.Empty;
            }

            result.Add(new BusListItemDto(
                bus.Id,
                bus.Plate,
                driverName,
                string.Empty,
                string.Empty));
        }

        return result;
    }
}
