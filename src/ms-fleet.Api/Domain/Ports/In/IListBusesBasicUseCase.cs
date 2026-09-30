using ms_fleet.Api.Application.Dtos;

namespace ms_fleet.Api.Domain.Ports.In;

public interface IListBusesBasicUseCase
{
    Task<IReadOnlyList<BusListItemDto>> ExecuteAsync(CancellationToken ct = default);
}
