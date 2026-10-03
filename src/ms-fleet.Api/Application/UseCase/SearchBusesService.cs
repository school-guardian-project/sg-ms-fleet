using ms_fleet.Api.Application.Dtos;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class SearchBusesService
{
    private readonly IListBusesBasicUseCase _listBusesBasicUseCase;
    private readonly IEnumerable<IBusSearchStrategy> _strategies;

    public SearchBusesService(
        IListBusesBasicUseCase listBusesBasicUseCase,
        IEnumerable<IBusSearchStrategy> strategies)
    {
        _listBusesBasicUseCase = listBusesBasicUseCase;
        _strategies = strategies;
    }

    public async Task<IReadOnlyList<BusListItemDto>> SearchAsync(string? search, CancellationToken ct = default)
    {
        search = search?.Trim();

        if (string.IsNullOrEmpty(search)) return [];

        var strategy = _strategies.FirstOrDefault(x => x.CanHandle(search));

        if (strategy is null) return [];

        var buses = await _listBusesBasicUseCase.ExecuteAsync(ct);

        return buses.Where(bus => strategy.Matches(bus, search)).ToList();
    }
}
