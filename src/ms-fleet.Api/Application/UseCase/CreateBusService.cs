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
        CancellationToken ct = default)
    {
        if (await _busRepository.ExistsByPlateAsync(plate, ct))
            throw new ArgumentException("Plate already exists");

        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero");

        if (!await _gpsDeviceService.GpsDeviceExistsAsync(gpsDeviceId, ct))
            throw new ArgumentException("GPS device not found");

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
        return bus.Id;
    }
}
