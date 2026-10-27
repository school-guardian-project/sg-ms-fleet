using ms_fleet.Api.Application.Security;
using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Tests.Fakes;

/// <summary>Politicas de acceso preconstruidas para los tests de casos de uso.</summary>
public static class AccessPolicies
{
    /// <summary>SuperAdmin con buses.read: sin filtro de tenant ni de pertenencia.</summary>
    public static BusAccessPolicy SuperAdmin() => new(
        new FakeCurrentUser { ProfileId = Guid.NewGuid(), Permissions = [FleetPermissions.BusesRead] },
        new FakeTenantProvider { RoleId = FleetRoles.SuperAdmin },
        new InMemoryCampusReferenceRepository());
}
