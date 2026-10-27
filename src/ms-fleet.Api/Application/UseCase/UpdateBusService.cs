using ms_fleet.Api.Application.Dto.Request;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

using ms_fleet.Api.Application.Security;

namespace ms_fleet.Api.Application.UseCase;

public class UpdateBusService : IUpdateBusUseCase
{
    private readonly IBusRepository _busRepository;
    private readonly IGpsDeviceService _gpsDeviceService;
    private readonly IBusRequestMapper _requestMapper;
    private readonly BusAccessPolicy _accessPolicy;

    public UpdateBusService(
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

    public async Task ExecuteAsync(Guid busId, UpdateBusRequest request, CancellationToken ct = default)
    {
        var bus = await _busRepository.GetByIdAsync(busId, ct)
            ?? throw new InvalidOperationException("Bus not found");

        await _accessPolicy.EnsureCanManageBusAsync(bus, ct);

        // Mover el bus a otra sede: la de destino tambien debe ser del colegio.
        if (request.CampuseId is { } targetCampus && targetCampus != Guid.Empty && targetCampus != bus.CampuseId)
            await _accessPolicy.EnsureCanUseCampusAsync(targetCampus, ct);

        if (request.Capacity.HasValue && request.Capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero");

        string? newPlate = request.Plate?.Trim();
        if (!string.IsNullOrEmpty(newPlate)
            && !string.Equals(newPlate, bus.Plate, StringComparison.OrdinalIgnoreCase)
            && await _busRepository.ExistsByPlateAsync(newPlate, ct))
            throw new ArgumentException("Plate already exists");

        Guid? gpsDeviceId = request.GpsDeviceId;
        if ((gpsDeviceId is null || gpsDeviceId == Guid.Empty) && !string.IsNullOrWhiteSpace(request.GpsImei))
            gpsDeviceId = await _gpsDeviceService.ResolveByImeiAsync(request.GpsImei, ct);

        Guid? resolvedGpsId = gpsDeviceId;
        if (resolvedGpsId is { } gpsId && gpsId != Guid.Empty && gpsId != bus.GpsDeviceId)
        {
            var device = await _gpsDeviceService.GetAsync(gpsId, ct)
                ?? throw new ArgumentException("GPS device not found");
            if (device.AssignedBusId is not null && device.AssignedBusId != bus.Id)
                throw new ArgumentException("GPS device already assigned");
        }

        _requestMapper.UpdateDomain(bus, request);

        if (resolvedGpsId is { } resolvedId && resolvedId != Guid.Empty)
            bus.GpsDeviceId = resolvedId;

        await _busRepository.UpdateAsync(bus, ct);

        if (request.GpsStatus is not null)
            await _gpsDeviceService.SetStatusAsync(bus.GpsDeviceId, request.GpsStatus.Value, ct);
    }
}