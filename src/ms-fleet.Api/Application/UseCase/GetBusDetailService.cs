using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Application.Dtos;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class GetBusDetailService : IGetBusDetailUseCase
{
    private readonly IBusRepository _busRepository;
    private readonly IDriverAssignmentRepository _assignmentRepository;
    private readonly IIamService _iamService;

    public GetBusDetailService(
        IBusRepository busRepository,
        IDriverAssignmentRepository assignmentRepository,
        IIamService iamService)
    {
        _busRepository = busRepository;
        _assignmentRepository = assignmentRepository;
        _iamService = iamService;
    }

    public async Task<BusDetailDto?> ExecuteAsync(Guid busId, CancellationToken ct = default)
    {
        var bus = await _busRepository.GetByIdAsync(busId, ct);
        if (bus is null) return null;

        var assignment = await _assignmentRepository.GetActiveByBusIdAsync(busId, ct);
        string driverName = string.Empty;

        if (assignment != null)
        {
            driverName = await _iamService.GetDriverNameAsync(assignment.ProfileId, ct) ?? string.Empty;
        }

        var names = await _busRepository.GetModelNamesAsync(bus.ModelId, ct);

        return new BusDetailDto(
            bus.Id,
            bus.Plate,
            bus.CampuseId,
            bus.SoatValidity,
            bus.GpsDeviceId,
            bus.Capacity,
            bus.ModelId,
            bus.Status.ToString(),
            driverName,
            names?.Brand ?? string.Empty,
            names?.Model ?? string.Empty);
    }
}
