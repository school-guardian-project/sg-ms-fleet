namespace ms_fleet.Api.Domain.Model;

/// <summary>
/// Permisos del catalogo Iam.Permissions que gobiernan los endpoints de flota
/// (FR-2.1). Viajan en el claim "permissions" del access token.
/// </summary>
public static class FleetPermissions
{
    public const string BusesRead = "buses.read";
    public const string BusesManage = "buses.manage";
    public const string DriversManage = "drivers.manage";
}
