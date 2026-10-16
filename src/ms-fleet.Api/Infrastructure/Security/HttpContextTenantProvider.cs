using System.Security.Claims;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Infrastructure.Security;

/// <summary>
/// Resuelve el tenant desde las claims del usuario autenticado (HttpContext.User).
/// Sin token o con token inválido la petición llega anónima y ShouldFilter es false,
/// de modo que las llamadas internas (p. ej. ms-route → /api/buses/assigned/{profileId})
/// no se filtran.
/// </summary>
public sealed class HttpContextTenantProvider : ITenantProvider
{
    private const string RoleIdClaim = "roleId";
    private const string CampusIdClaim = "campusId";
    private const string SchoolIdClaim = "schoolId";

    public int? RoleId { get; }
    public Guid? CampusId { get; }
    public Guid? SchoolId { get; }
    public bool ShouldFilter { get; }

    public HttpContextTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true) return;

        RoleId = ParseInt(user, RoleIdClaim);
        CampusId = ParseGuid(user, CampusIdClaim);
        SchoolId = ParseGuid(user, SchoolIdClaim);

        ShouldFilter = RoleId switch
        {
            1 => SchoolId is not null,           // Admin: sedes de su colegio
            2 or 3 or 4 => CampusId is not null, // Student/Driver/Parent: solo su sede
            _ => false                           // SuperAdmin (5) u otros roles: sin filtro
        };
    }

    private static int? ParseInt(ClaimsPrincipal user, string claim)
        => int.TryParse(user.FindFirstValue(claim), out var value) ? value : null;

    private static Guid? ParseGuid(ClaimsPrincipal user, string claim)
        => Guid.TryParse(user.FindFirstValue(claim), out var value) ? value : null;
}
