using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Infrastructure.External;

public class IamService : IIamService
{
    private readonly HttpClient _httpClient;

    public IamService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ProfileExistsAsync(Guid profileId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/profiles/{profileId}/exists", ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GetDriverNameAsync(Guid profileId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/profiles/{profileId}/name", ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadAsStringAsync(ct);
    }
}
