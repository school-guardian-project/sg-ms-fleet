using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class ChangeBusStatusService : IChangeBusStatusUseCase
{
    private readonly IBusRepository _busRepository;

    public ChangeBusStatusService(IBusRepository busRepository)
    {
        _busRepository = busRepository;
    }

    public async Task ExecuteAsync(Guid busId, Status status, CancellationToken ct = default)
    {
        var bus = await _busRepository.GetByIdAsync(busId, ct)
            ?? throw new InvalidOperationException("Bus not found");

        bus.Status = status;
        await _busRepository.UpdateAsync(bus, ct);
    }
}
