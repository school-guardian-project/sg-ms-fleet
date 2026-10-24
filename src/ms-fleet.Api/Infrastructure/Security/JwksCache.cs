using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ms_fleet.Api.Infrastructure.Security;

/// <summary>
/// Caches public keys from the IAM JWKS endpoint to enable local RS256 validation.
/// </summary>
public class JwksCache
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<JwksCache> _logger;
    private readonly string _jwksUrl;

    public JwksCache(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        ILogger<JwksCache> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _logger = logger;
        _jwksUrl = configuration["Iam:JwksUrl"] ?? "http://ms-iam:8080/.well-known/jwks.json";
    }

    public async Task<string> GetPublicKeyAsync(string kid)
    {
        var keys = await GetKeysAsync();
        var key = keys.FirstOrDefault(k => k.Kid == kid);
        if (key == null)
        {
            _logger.LogWarning("JWK with kid {Kid} not found in cached set", kid);
            return null;
        }
        return key.PublicKey;
    }

    private async Task<List<JwkKey>> GetKeysAsync()
    {
        return await _cache.GetOrCreateAsync("jwks_keys", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
            try
            {
                var client = _httpClientFactory.CreateClient();
                var jwks = await client.GetFromJsonAsync<JwksResponse>(_jwksUrl);
                _logger.LogInformation("JWKS successfully fetched from {Url}", _jwksUrl);
                return jwks?.Keys ?? new List<JwkKey>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch JWKS from {Url}", _jwksUrl);
                return new List<JwkKey>();
            }
        });
    }

    private class JwksResponse
    {
        public List<JwkKey> Keys { get; set; } = new();
    }

    private class JwkKey
    {
        public string Kid { get; set; } = string.Empty;
        public string N { get; set; } = string.Empty;
        public string E { get; set; } = string.Empty;
        // Simplified: store as raw components or convert to PEM.
        // In actual implementation, we'd use a library like Microsoft.IdentityModel.Tokens
        public string PublicKey => $"n={N},e={E}";
    }
}
