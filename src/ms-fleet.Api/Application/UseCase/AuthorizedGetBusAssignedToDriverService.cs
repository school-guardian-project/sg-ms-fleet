using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Application.Security;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.In;

namespace ms_fleet.Api.Application.UseCase;

/// <summary>
/// Decorador de <see cref="IFindBusAssignedToDriverQuery"/> para los clientes REST:
/// aplica la politica de acceso antes y despues de la consulta. Separarlo permite
/// que el gRPC interno, que no trae usuario, use la consulta sin la politica.
/// </summary>
public sealed class AuthorizedGetBusAssignedToDriverService : IGetBusAssignedToDriverUseCase
{
    private readonly IFindBusAssignedToDriverQuery _inner;
    private readonly BusAccessPolicy _accessPolicy;

    public AuthorizedGetBusAssignedToDriverService(IFindBusAssignedToDriverQuery inner, BusAccessPolicy accessPolicy)
    {
        _inner = inner;
        _accessPolicy = accessPolicy;
    }

    /// <exception cref="BusAccessDeniedException">
    /// El perfil autenticado no es ese conductor ni personal con buses.read.
    /// </exception>
    public async Task<BusAssignedResponse?> ExecuteAsync(Guid profileId, CancellationToken ct = default)
    {
        _accessPolicy.EnsureCanReadDriverBus(profileId);

        var bus = await _inner.ExecuteAsync(profileId, ct);
        if (bus is null) return null;

        // El personal solo ve los buses de su colegio; el propio conductor siempre ve el suyo.
        if (!_accessPolicy.IsSelf(profileId) && !await _accessPolicy.CanSeeCampusAsync(bus.CampuseId, ct))
            return null;

        return bus;
    }
}
