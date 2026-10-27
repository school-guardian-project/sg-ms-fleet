using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Tests.Fakes;

public class FakeCurrentUser : ICurrentUser
{
    public Guid? ProfileId { get; init; }
    public IReadOnlyCollection<string> Permissions { get; init; } = [];

    public bool HasPermission(string permission) => Permissions.Contains(permission);
}
