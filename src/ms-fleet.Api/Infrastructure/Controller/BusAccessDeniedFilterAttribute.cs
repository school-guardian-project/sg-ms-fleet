using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Infrastructure.Controller;

/// <summary>
/// Traduce <see cref="BusAccessDeniedException"/> (regla de pertenencia de la capa
/// de aplicacion) a 403 ProblemDetails, en un solo lugar para todo el controlador.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class BusAccessDeniedFilterAttribute : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not BusAccessDeniedException ex) return;

        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Detail = ex.Message,
        })
        {
            StatusCode = StatusCodes.Status403Forbidden,
        };
        context.ExceptionHandled = true;
    }
}
