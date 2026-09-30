using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class ListBusesService : IListBusesUseCase
{
    private readonly IBusRepository _busRepository;

    public ListBusesService(IBusRepository busRepository)
    {
        _busRepository = busRepository;
    }

    public Task<IReadOnlyList<Bus>> ExecuteAsync(CancellationToken ct = default)
    {
        return _busRepository.GetAllAsync(ct);
    }
}
