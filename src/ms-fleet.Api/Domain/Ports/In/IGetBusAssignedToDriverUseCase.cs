using ms_fleet.Api.Application.Dto.Response;

namespace ms_fleet.Api.Domain.Ports.In;

public interface IGetBusAssignedToDriverUseCase
{
    Task<BusAssignedResponse?> ExecuteAsync(Guid profileId, CancellationToken ct = default);
}