using ms_fleet.Api.Application.Dto.Response;

namespace ms_fleet.Api.Domain.Ports.In;

public interface IGetBusDetailUseCase
{
    Task<BusDetailResponse?> ExecuteAsync(Guid busId, CancellationToken ct = default);
}