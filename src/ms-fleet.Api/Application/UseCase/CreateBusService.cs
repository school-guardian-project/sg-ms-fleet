using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class CreateBusService : ICreateBusUseCase
{
    private readonly IBusRepository _busRepository;
    private readonly IGpsDeviceService _gpsDeviceService;

    public CreateBusService(IBusRepository busRepository, IGpsDeviceService gpsDeviceService)
    {
        _busRepository = busRepository;
        _gpsDeviceService = gpsDeviceService;
    }

    public async Task<Guid> ExecuteAsync(
        Guid campuseId,
        DateTime soatValidity,
        Guid gpsDeviceId,
        byte capacity,
        string plate,
        int modelId,
        bool? gpsStatus = null,
        string? gpsImei = null,
        CancellationToken ct = default)
    {
        plate = plate?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(plate))
            throw new ArgumentException("Plate is required");

        if (await _busRepository.ExistsByPlateAsync(plate, ct))
            throw new ArgumentException("Plate already exists");

        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero");

        if (gpsDeviceId == Guid.Empty && !string.IsNullOrWhiteSpace(gpsImei))
            gpsDeviceId = await _gpsDeviceService.ResolveByImeiAsync(gpsImei, ct);

        if (gpsDeviceId == Guid.Empty || !await _gpsDeviceService.GpsDeviceExistsAsync(gpsDeviceId, ct))
            throw new ArgumentException("GPS device not found");

        var device = await _gpsDeviceService.GetAsync(gpsDeviceId, ct);
        if (device?.AssignedBusId is not null)
            throw new ArgumentException("GPS device already assigned");

        var bus = new Bus
        {
            CampuseId = campuseId,
            SoatValidity = soatValidity,
            GpsDeviceId = gpsDeviceId,
            Capacity = capacity,
            Plate = plate,
            ModelId = (byte)modelId,
            Status = Status.Active
        };

        await _busRepository.AddAsync(bus, ct);

        if (gpsStatus is not null)
            await _gpsDeviceService.SetStatusAsync(gpsDeviceId, gpsStatus.Value, ct);

        return bus.Id;
    }
}
