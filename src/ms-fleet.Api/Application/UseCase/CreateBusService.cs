using ms_fleet.Api.Application.Dto.Request;
using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

using ms_fleet.Api.Application.Security;

namespace ms_fleet.Api.Application.UseCase;

public class CreateBusService : ICreateBusUseCase
{
    private readonly IBusRepository _busRepository;
    private readonly IGpsDeviceService _gpsDeviceService;
    private readonly IBusRequestMapper _requestMapper;
    private readonly BusAccessPolicy _accessPolicy;

    public CreateBusService(
        IBusRepository busRepository,
        IGpsDeviceService gpsDeviceService,
        IBusRequestMapper requestMapper,
        BusAccessPolicy accessPolicy)
    {
        _busRepository = busRepository;
        _gpsDeviceService = gpsDeviceService;
        _requestMapper = requestMapper;
        _accessPolicy = accessPolicy;
    }

    public async Task<CreateBusResponse> ExecuteAsync(CreateBusRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Plate))
            throw new ArgumentException("Plate is required");

        await _accessPolicy.EnsureCanUseCampusAsync(request.CampuseId, ct);

        if (await _busRepository.ExistsByPlateAsync(request.Plate.Trim(), ct))
            throw new ArgumentException("Plate already exists");

        if (request.Capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero");

        Guid gpsDeviceId = request.GpsDeviceId;
        if (gpsDeviceId == Guid.Empty && !string.IsNullOrWhiteSpace(request.GpsImei))
            gpsDeviceId = await _gpsDeviceService.ResolveByImeiAsync(request.GpsImei, ct);

        if (gpsDeviceId == Guid.Empty || !await _gpsDeviceService.GpsDeviceExistsAsync(gpsDeviceId, ct))
            throw new ArgumentException("GPS device not found");

        var device = await _gpsDeviceService.GetAsync(gpsDeviceId, ct);
        if (device?.AssignedBusId is not null)
            throw new ArgumentException("GPS device already assigned");

        var bus = _requestMapper.ToDomain(request);
        bus.GpsDeviceId = gpsDeviceId;

        await _busRepository.AddAsync(bus, ct);

        if (request.GpsStatus is not null)
            await _gpsDeviceService.SetStatusAsync(gpsDeviceId, request.GpsStatus.Value, ct);

        return new CreateBusResponse(bus.Id);
    }
}