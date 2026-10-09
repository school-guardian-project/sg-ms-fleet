using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class UpdateBusService : IUpdateBusUseCase
{
    private readonly IBusRepository _busRepository;
    private readonly IGpsDeviceService _gpsDeviceService;

    public UpdateBusService(IBusRepository busRepository, IGpsDeviceService gpsDeviceService)
    {
        _busRepository = busRepository;
        _gpsDeviceService = gpsDeviceService;
    }

    public async Task ExecuteAsync(
        Guid busId,
        Guid campuseId,
        DateTime soatValidity,
        byte capacity,
        int modelId,
        string? plate = null,
        Guid? gpsDeviceId = null,
        bool? gpsStatus = null,
        string? gpsImei = null,
        CancellationToken ct = default)
    {
        var bus = await _busRepository.GetByIdAsync(busId, ct)
            ?? throw new InvalidOperationException("Bus not found");

        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero");

        var newPlate = plate?.Trim();
        if (!string.IsNullOrEmpty(newPlate)
            && !string.Equals(newPlate, bus.Plate, StringComparison.OrdinalIgnoreCase)
            && await _busRepository.ExistsByPlateAsync(newPlate, ct))
            throw new ArgumentException("Plate already exists");

        if ((gpsDeviceId is null || gpsDeviceId == Guid.Empty) && !string.IsNullOrWhiteSpace(gpsImei))
            gpsDeviceId = await _gpsDeviceService.ResolveByImeiAsync(gpsImei, ct);

        if (gpsDeviceId is { } gpsId && gpsId != Guid.Empty && gpsId != bus.GpsDeviceId)
        {
            var device = await _gpsDeviceService.GetAsync(gpsId, ct)
                ?? throw new ArgumentException("GPS device not found");
            if (device.AssignedBusId is not null && device.AssignedBusId != bus.Id)
                throw new ArgumentException("GPS device already assigned");
            bus.GpsDeviceId = gpsId;
        }

        // Campos que el cliente no envia conservan su valor actual.
        if (campuseId != Guid.Empty) bus.CampuseId = campuseId;
        if (soatValidity != default) bus.SoatValidity = soatValidity;
        if (modelId > 0) bus.ModelId = (byte)modelId;
        if (!string.IsNullOrEmpty(newPlate)) bus.Plate = newPlate;
        bus.Capacity = capacity;

        await _busRepository.UpdateAsync(bus, ct);

        if (gpsStatus is not null)
            await _gpsDeviceService.SetStatusAsync(bus.GpsDeviceId, gpsStatus.Value, ct);
    }
}
