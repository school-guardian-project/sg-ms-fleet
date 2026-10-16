using System.Security.Cryptography;
using System.Text;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ms_fleet.Api.Infrastructure.Grpc;

/// <summary>
/// gRPC counterpart of ms-iam's InternalApiKeyInterceptor: every incoming call
/// must carry the shared secret in the "x-internal-api-key" metadata header.
/// The key comes from configuration (Fleet:ApiKey), never hardcoded, and the
/// comparison is constant time so a timing oracle cannot recover it.
/// </summary>
public class InternalApiKeyInterceptor : Interceptor
{
    public const string HeaderName = "x-internal-api-key";

    private readonly byte[] _expectedKey;

    public InternalApiKeyInterceptor(IConfiguration configuration)
    {
        var apiKey = configuration["Fleet:ApiKey"];
        _expectedKey = string.IsNullOrEmpty(apiKey)
            ? Array.Empty<byte>()
            : Encoding.UTF8.GetBytes(apiKey);
    }

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        // Sin clave configurada o sin clave enviada: rechazar (fail closed).
        if (!IsValidKey(context.RequestHeaders.GetValue(HeaderName)))
        {
            throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                "Invalid or missing internal API key"));
        }

        return continuation(request, context);
    }

    /// <summary>
    /// Validates the key sent by the caller. Exposed separately from the
    /// interceptor plumbing so the fail-closed rules can be unit tested.
    /// </summary>
    public bool IsValidKey(string? provided)
    {
        if (provided is null || _expectedKey.Length == 0) return false;

        var providedKey = Encoding.UTF8.GetBytes(provided);
        return CryptographicOperations.FixedTimeEquals(providedKey, _expectedKey);
    }
}
