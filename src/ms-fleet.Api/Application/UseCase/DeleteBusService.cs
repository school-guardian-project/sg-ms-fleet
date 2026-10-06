using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class DeleteBusService : IDeleteBusUseCase
{
    private readonly IBusRepository _busRepository;

    public DeleteBusService(IBusRepository busRepository)
    {
        _busRepository = busRepository;
    }

    public async Task ExecuteAsync(Guid busId, CancellationToken ct = default)
    {
        var bus = await _busRepository.GetByIdAsync(busId, ct)
            ?? throw new InvalidOperationException("Bus not found");

        bus.Status = Status.Inactive;
        await _busRepository.UpdateAsync(bus, ct);
    }
}
