using ms_fleet.Api.Application.Dtos;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class GetBusAssignedToDriverService : IGetBusAssignedToDriverUseCase
{
    private readonly IDriverAssignmentRepository _assignmentRepository;
    private readonly IBusRepository _busRepository;

    public GetBusAssignedToDriverService(
        IDriverAssignmentRepository assignmentRepository,
        IBusRepository busRepository)
    {
        _assignmentRepository = assignmentRepository;
        _busRepository = busRepository;
    }

    public async Task<BusAssignedDto?> ExecuteAsync(Guid profileId, CancellationToken ct = default)
    {
        var assignment = await _assignmentRepository.GetActiveByProfileIdAsync(profileId, ct);
        if (assignment is null) return null;

        var bus = await _busRepository.GetByIdAsync(assignment.BusId, ct);
        if (bus is null || bus.Status != Status.Active) return null;

        return new BusAssignedDto(bus.Id, bus.Plate, bus.CampuseId);
    }
}
