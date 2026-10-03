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
        try
        {
            var response = await _httpClient.GetAsync($"api/profiles/{profileId}/exists", ct);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            // ms-iam caído no debe tumbar la asignación de conductores.
            return false;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return false;
        }
    }

    public async Task<string?> GetDriverNameAsync(Guid profileId, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/profiles/{profileId}/name", ct);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsStringAsync(ct);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return null;
        }
    }
}
