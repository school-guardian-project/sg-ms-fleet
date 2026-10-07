using ms_fleet.Api.Application.Dtos;

namespace ms_fleet.Api.Domain.Ports.In;

public interface IGetBusAssignedToDriverUseCase
{
    Task<BusAssignedDto?> ExecuteAsync(Guid profileId, CancellationToken ct = default);
}
