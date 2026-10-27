namespace ms_fleet.Api.Domain.Model;

/// <summary>Roles del catalogo Iam.Role (claim "roleId").</summary>
public static class FleetRoles
{
    public const int Admin = 1;
    public const int SuperAdmin = 5;

    /// <summary>Personal del colegio: puede consultar datos de otros perfiles.</summary>
    public static bool IsStaff(int? roleId) => roleId is Admin or SuperAdmin;
}
