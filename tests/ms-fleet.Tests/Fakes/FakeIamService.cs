using ms_fleet.Api.Domain.Ports.Out;
using Xunit;
using Xunit;
namespace ms_fleet.Api.Tests.Fakes;

public class FakeIamService : IIamService
{
    public Task<bool> ProfileExistsAsync(Guid profileId, CancellationToken ct = default)
        => Task.FromResult(true);
}
