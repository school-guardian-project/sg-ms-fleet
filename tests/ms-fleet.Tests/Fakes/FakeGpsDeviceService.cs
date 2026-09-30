namespace ms_fleet.Api.Tests.Fakes;

public class FakeGpsDeviceService : IGpsDeviceService
{
    public Task<bool> GpsDeviceExistsAsync(Guid gpsDeviceId, CancellationToken ct = default)
        => Task.FromResult(true);
}
