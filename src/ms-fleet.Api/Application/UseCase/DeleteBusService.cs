using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

using ms_fleet.Api.Application.Security;

namespace ms_fleet.Api.Application.UseCase;

public class DeleteBusService : IDeleteBusUseCase
{
    private readonly IBusRepository _busRepository;
    private readonly BusAccessPolicy _accessPolicy;

    public DeleteBusService(IBusRepository busRepository,
        BusAccessPolicy accessPolicy)
    {
        _busRepository = busRepository;
        _accessPolicy = accessPolicy;
    }

    public async Task ExecuteAsync(Guid busId, CancellationToken ct = default)
    {
        var bus = await _busRepository.GetByIdAsync(busId, ct)
            ?? throw new InvalidOperationException("Bus not found");

        await _accessPolicy.EnsureCanManageBusAsync(bus, ct);

        bus.Status = Status.Inactive;
        await _busRepository.UpdateAsync(bus, ct);
    }
}
