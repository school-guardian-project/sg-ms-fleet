using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Domain.Ports.Out;

public interface IGpsDeviceService
{
    Task<bool> GpsDeviceExistsAsync(Guid gpsDeviceId, CancellationToken ct = default);
    Task<GpsDeviceInfo?> GetAsync(Guid gpsDeviceId, CancellationToken ct = default);
    Task<IReadOnlyList<GpsDeviceInfo>> ListAsync(CancellationToken ct = default);
    Task SetStatusAsync(Guid gpsDeviceId, bool active, CancellationToken ct = default);

    /// <summary>Id del GPS con ese IMEI; si no existe lo registra.</summary>
    Task<Guid> ResolveByImeiAsync(string imei, CancellationToken ct = default);
}