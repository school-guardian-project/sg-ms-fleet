using ms_fleet.Api.Application.Dtos;

namespace ms_fleet.Api.Domain.Ports.In;

public interface IGetBusDetailUseCase
{
    Task<BusDetailDto?> ExecuteAsync(Guid busId, CancellationToken ct = default);
}
