using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class GetBusService : IGetBusUseCase
{
    private readonly IBusRepository _busRepository;

    public GetBusService(IBusRepository busRepository)
    {
        _busRepository = busRepository;
    }

    public Task<Bus?> ExecuteAsync(Guid busId, CancellationToken ct = default)
    {
        return _busRepository.GetByIdAsync(busId, ct);
    }
}
