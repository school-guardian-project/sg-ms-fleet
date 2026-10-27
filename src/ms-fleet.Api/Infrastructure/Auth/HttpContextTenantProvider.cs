using System.Security.Claims;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Infrastructure.Auth;

/// <summary>
/// Resuelve el tenant desde las claims del usuario autenticado (HttpContext.User).
/// Falla cerrado: todo usuario autenticado se filtra salvo el SuperAdmin (roleId 5),
/// tambien si el token no trae roleId o trae un rol desconocido. Sin usuario (el
/// gRPC interno, autenticado por API key) no hay tenant y no se filtra.
/// </summary>
public sealed class HttpContextTenantProvider : ITenantProvider
{
    public int? RoleId { get; }
    public Guid? CampusId { get; }
    public Guid? SchoolId { get; }
    public bool ShouldFilter { get; }

    public HttpContextTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true) return;

        RoleId = ParseInt(user, SgClaimTypes.RoleId);
        CampusId = ParseGuid(user, SgClaimTypes.CampusId);
        SchoolId = ParseGuid(user, SgClaimTypes.SchoolId);

        // Admin: sedes de su colegio; Student/Driver/Parent: su sede. Sin colegio o
        // sede resolubles BusTenantScope no devuelve ninguna sede visible.
        ShouldFilter = RoleId != FleetRoles.SuperAdmin;
    }

    private static int? ParseInt(ClaimsPrincipal user, string claim)
        => int.TryParse(user.FindFirstValue(claim), out var value) ? value : null;

    private static Guid? ParseGuid(ClaimsPrincipal user, string claim)
        => Guid.TryParse(user.FindFirstValue(claim), out var value) ? value : null;
}
