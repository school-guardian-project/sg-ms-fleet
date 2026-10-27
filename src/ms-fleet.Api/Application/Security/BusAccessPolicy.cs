using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.Security;

/// <summary>
/// Reglas de pertenencia de los datos de flota para el perfil autenticado.
/// Complementan los permisos por endpoint: el permiso dice que operacion puede
/// hacer, la politica dice sobre que buses y conductores.
/// </summary>
public sealed class BusAccessPolicy
{
    private readonly ICurrentUser _currentUser;
    private readonly ITenantProvider _tenantProvider;
    private readonly BusTenantScope _scope;

    public BusAccessPolicy(
        ICurrentUser currentUser,
        ITenantProvider tenantProvider,
        ICampusReferenceRepository campusReferenceRepository)
    {
        _currentUser = currentUser;
        _tenantProvider = tenantProvider;
        _scope = new BusTenantScope(tenantProvider, campusReferenceRepository);
    }

    /// <summary>
    /// El bus asignado a un conductor lo ve el propio conductor, o el personal
    /// del colegio (Admin/SuperAdmin) con buses.read. Un conductor tiene
    /// buses.read pero no puede consultar el bus de otro conductor.
    /// </summary>
    public void EnsureCanReadDriverBus(Guid driverProfileId)
    {
        if (IsSelf(driverProfileId)) return;

        if (FleetRoles.IsStaff(_tenantProvider.RoleId) && _currentUser.HasPermission(FleetPermissions.BusesRead))
            return;

        throw new BusAccessDeniedException();
    }

    public bool IsSelf(Guid profileId) => _currentUser.ProfileId == profileId;

    /// <summary>El bus pertenece a una sede visible para el tenant del token.</summary>
    public Task<bool> CanSeeBusAsync(Bus bus, CancellationToken ct = default) =>
        _scope.CanSeeCampusAsync(bus.CampuseId, ct);
}
