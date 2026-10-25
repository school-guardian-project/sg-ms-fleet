using Microsoft.AspNetCore.Authentication;

namespace ms_fleet.Api.Infrastructure.Auth;

/// <summary>
/// Opciones del esquema de autenticación basado en las cabeceras del gateway.
/// </summary>
public sealed class GatewayAuthenticationOptions : AuthenticationSchemeOptions
{
}