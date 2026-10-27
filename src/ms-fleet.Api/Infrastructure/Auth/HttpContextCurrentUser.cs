using System.Security.Claims;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Infrastructure.Auth;

/// <summary>Adaptador de <see cref="ICurrentUser"/> sobre HttpContext.User.</summary>
public sealed class HttpContextCurrentUser : ICurrentUser
{
    private readonly ClaimsPrincipal? _user;

    public HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        _user = user?.Identity?.IsAuthenticated == true ? user : null;
    }

    public Guid? ProfileId =>
        Guid.TryParse(_user?.FindFirstValue(SgClaimTypes.Subject), out var id) ? id : null;

    public bool HasPermission(string permission) =>
        _user?.HasClaim(SgClaimTypes.Permissions, permission) == true;
}
