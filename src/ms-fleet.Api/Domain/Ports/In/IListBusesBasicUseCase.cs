using ms_fleet.Api.Application.Dto.Response;

namespace ms_fleet.Api.Domain.Ports.In;

public interface IListBusesBasicUseCase
{
    Task<IReadOnlyList<BusListItemResponse>> ExecuteAsync(CancellationToken ct = default);
}