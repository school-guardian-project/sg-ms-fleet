using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace ms_fleet.Api.Tests.Infrastructure;

/// <summary>
/// Minimal ServerCallContext so the gRPC service can be unit tested without
/// hosting a real server. Only the members the tests exercise are meaningful:
/// the request headers (internal API key), the deadline and the cancellation token.
/// </summary>
public class TestServerCallContext : ServerCallContext
{
    private readonly CancellationToken _cancellationToken;

    public TestServerCallContext(Metadata? requestHeaders = null, CancellationToken cancellationToken = default)
    {
        RequestHeadersCore = requestHeaders ?? new Metadata();
        _cancellationToken = cancellationToken;
    }

    protected override string MethodCore => "/route.v1.RouteService/GetBusDetails";
    protected override string HostCore => "localhost";
    protected override string PeerCore => "ipv4:127.0.0.1:5001";
    protected override DateTime DeadlineCore => DateTime.UtcNow.AddMinutes(1);
    protected override Metadata RequestHeadersCore { get; }
    protected override Metadata ResponseTrailersCore { get; } = new();
    protected override Status StatusCore { get; set; }
    protected override WriteOptions? WriteOptionsCore { get; set; }
    protected override CancellationToken CancellationTokenCore => _cancellationToken;
    protected override IDictionary<object, object> UserStateCore { get; } = new Dictionary<object, object>();

    protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
        => Task.CompletedTask;

    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options)
        => throw new NotSupportedException("Propagation is not exercised by these tests.");

    protected override AuthContext AuthContextCore
        => throw new NotSupportedException("Auth is not exercised by these tests.");
}
