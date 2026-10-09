using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Tests.Fakes;

public class FakeGpsDeviceService : IGpsDeviceService
{
    public readonly Dictionary<Guid, GpsDeviceInfo> Devices = new();

    public Task<bool> GpsDeviceExistsAsync(Guid gpsDeviceId, CancellationToken ct = default)
        => Task.FromResult(true);

    public Task<GpsDeviceInfo?> GetAsync(Guid gpsDeviceId, CancellationToken ct = default)
        => Task.FromResult(Devices.TryGetValue(gpsDeviceId, out var d) ? d : null);

    public Task<IReadOnlyList<GpsDeviceInfo>> ListAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<GpsDeviceInfo>>(Devices.Values.ToList());

    public Task<Guid> ResolveByImeiAsync(string imei, CancellationToken ct = default)
    {
        var found = Devices.Values.FirstOrDefault(d => d.Imei == imei);
        if (found is not null) return Task.FromResult(found.Id);
        var id = Guid.NewGuid();
        Devices[id] = new GpsDeviceInfo(id, imei, true, null, string.Empty);
        return Task.FromResult(id);
    }

    public Task SetStatusAsync(Guid gpsDeviceId, bool active, CancellationToken ct = default)
    {
        if (Devices.TryGetValue(gpsDeviceId, out var d)) Devices[gpsDeviceId] = d with { GpsStatus = active };
        return Task.CompletedTask;
    }
}
