using ms_fleet.Api.Domain.Ports.Out;
using Xunit;

namespace ms_fleet.Api.Tests.Fakes;

public class FakeIamService : IIamService
{
    public Task<bool> ProfileExistsAsync(Guid profileId, CancellationToken ct = default)
        => Task.FromResult(true);

    public Task<string?> GetDriverNameAsync(Guid profileId, CancellationToken ct = default)
        => Task.FromResult<string?>("Test Driver");
}
