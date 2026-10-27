using ms_fleet.Api.Application.Dto.Response;

namespace ms_fleet.Api.Domain.Ports.In;

/// <summary>
/// Bus activo asignado a un conductor, sin reglas de quien pregunta. Es el puerto
/// del gRPC interno (ms-route), que no trae usuario y ya viene autenticado por la
/// API key. Los clientes REST usan <see cref="IGetBusAssignedToDriverUseCase"/>,
/// que aplica la politica de acceso sobre esta consulta.
/// </summary>
public interface IFindBusAssignedToDriverQuery
{
    Task<BusAssignedResponse?> ExecuteAsync(Guid profileId, CancellationToken ct = default);
}
