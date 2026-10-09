namespace ms_fleet.Api.Domain.Ports.Out;

/// <summary>
/// Información del tenant (colegio/sede) de la petición actual, derivada del JWT.
/// </summary>
public interface ITenantProvider
{
    int? RoleId { get; }
    Guid? CampusId { get; }
    Guid? SchoolId { get; }

    /// <summary>
    /// True cuando el JWT identifica un tenant cuyas listas de buses deben filtrarse.
    /// Sin token (llamadas internas) o SuperAdmin (roleId 5) → false.
    /// </summary>
    bool ShouldFilter { get; }
}
