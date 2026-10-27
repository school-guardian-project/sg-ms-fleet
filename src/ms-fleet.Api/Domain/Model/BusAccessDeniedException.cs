namespace ms_fleet.Api.Domain.Model;

/// <summary>
/// El perfil autenticado no puede consultar el recurso pedido (p. ej. un
/// conductor que pide el bus asignado a otro conductor).
/// </summary>
public sealed class BusAccessDeniedException : Exception
{
    public BusAccessDeniedException()
        : base("The authenticated profile cannot access this resource.")
    {
    }
}
