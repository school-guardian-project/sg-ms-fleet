using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Tests.Fakes;

public class FakeTenantProvider : ITenantProvider
{
    public int? RoleId { get; init; }
    public Guid? CampusId { get; init; }
    public Guid? SchoolId { get; init; }
    public bool ShouldFilter { get; init; }
}
