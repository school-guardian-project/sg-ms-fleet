namespace ms_fleet.Api.Domain.Ports.Out;

public interface IGpsDeviceService
{
    Task<bool> GpsDeviceExistsAsync(Guid gpsDeviceId, CancellationToken ct = default);
}
