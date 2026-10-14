using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Infrastructure.Grpc;

/// <summary>
/// gRPC client for ms-iam's profile service (contract usermanagement.v1.ProfileService).
/// Inter-service traffic is gRPC-only (ADR-005); the shared secret travels in
/// the x-internal-api-key metadata header and comes from configuration.
/// </summary>
public sealed class IamGrpcClient : IIamService, IDisposable
{
    // Holgura amplia: la primera llamada tras un arranque en frio puede tardar
    // varios segundos en el servicio destino.
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);

    private readonly ProfileService.ProfileServiceClient _client;
    private readonly string _apiKey;
    private readonly GrpcChannel? _channel;

    public IamGrpcClient(IConfiguration configuration)
    {
        var address = configuration["Iam:GrpcAddress"]
            ?? throw new InvalidOperationException("Missing Iam:GrpcAddress (set it in .env)");
        _channel = GrpcChannel.ForAddress(address);
        _client = new ProfileService.ProfileServiceClient(_channel);
        _apiKey = configuration["Iam:ApiKey"] ?? string.Empty;
    }

    public async Task<bool> ProfileExistsAsync(Guid profileId, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.ProfileExistsAsync(
                new ProfileExistsRequest { ProfileId = profileId.ToString() },
                new Metadata { { "x-internal-api-key", _apiKey } }, Deadline(), ct);
            return response.Exists;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // Paridad con el puente REST anterior: 404 desde iam => no existe.
            return false;
        }
    }

    public async Task<string?> GetDriverNameAsync(Guid profileId, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetDriverNameAsync(
                new GetDriverNameRequest { ProfileId = profileId.ToString() },
                new Metadata { { "x-internal-api-key", _apiKey } }, Deadline(), ct);
            return response.Name;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // Paridad con el puente REST anterior: 404 desde iam => no existe.
            return null;
        }
    }

    private static DateTime Deadline() => DateTime.UtcNow.Add(Timeout);

    public void Dispose() => _channel?.Dispose();
}