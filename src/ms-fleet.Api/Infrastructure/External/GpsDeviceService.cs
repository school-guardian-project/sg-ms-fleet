using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Infrastructure.External;

public class GpsDeviceService : IGpsDeviceService
{
    private readonly HttpClient _httpClient;

    public GpsDeviceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> GpsDeviceExistsAsync(Guid gpsDeviceId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/gps-devices/{gpsDeviceId}/exists", ct);
        return response.IsSuccessStatusCode;
    }
}
