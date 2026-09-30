namespace ms_fleet.Api.Tests.Fakes;

public class FakeIamService : IIamService
{
    public Task<bool> ProfileExistsAsync(Guid profileId, CancellationToken ct = default)
        => Task.FromResult(true);
}
