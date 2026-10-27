using ms_fleet.Api.Application.Security;
using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class GetBusDetailService : IGetBusDetailUseCase
{
    private readonly IBusRepository _busRepository;
    private readonly IDriverAssignmentRepository _assignmentRepository;
    private readonly IIamService _iamService;
    private readonly IGpsDeviceService _gpsDeviceService;
    private readonly IBusMapper _busMapper;
    private readonly BusAccessPolicy _accessPolicy;

    public GetBusDetailService(
        IBusRepository busRepository,
        IDriverAssignmentRepository assignmentRepository,
        IIamService iamService,
        IGpsDeviceService gpsDeviceService,
        IBusMapper busMapper,
        BusAccessPolicy accessPolicy)
    {
        _accessPolicy = accessPolicy;
        _busRepository = busRepository;
        _assignmentRepository = assignmentRepository;
        _iamService = iamService;
        _gpsDeviceService = gpsDeviceService;
        _busMapper = busMapper;
    }

    public async Task<BusDetailResponse?> ExecuteAsync(Guid busId, CancellationToken ct = default)
    {
        var bus = await _busRepository.GetByIdAsync(busId, ct);
        // Un bus de otro colegio responde igual que uno inexistente (no revela que existe).
        if (bus is null || !await _accessPolicy.CanSeeBusAsync(bus, ct)) return null;

        var assignment = await _assignmentRepository.GetActiveByBusIdAsync(busId, ct);
        string driverName = string.Empty;

        if (assignment != null)
        {
            driverName = await _iamService.GetDriverNameAsync(assignment.ProfileId, ct) ?? string.Empty;
        }

        var gps = await _gpsDeviceService.GetAsync(bus.GpsDeviceId, ct);
        var names = await _busRepository.GetModelNamesAsync(bus.ModelId, ct);

        return _busMapper.ToDetail(bus, driverName, names, gps, assignment?.ProfileId);
    }
}