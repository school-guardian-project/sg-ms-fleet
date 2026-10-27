namespace ms_fleet.Api.Domain.Ports.Out;

/// <summary>
/// Identidad del perfil autenticado en la peticion actual, tomada del access
/// token validado.
/// </summary>
public interface ICurrentUser
{
    /// <summary>ProfileId (claim "sub"), o null sin usuario autenticado.</summary>
    Guid? ProfileId { get; }

    bool HasPermission(string permission);
}
