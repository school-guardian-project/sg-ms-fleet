using ms_fleet.Api.Application.Dto.Request;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

using ms_fleet.Api.Application.Security;

namespace ms_fleet.Api.Application.UseCase;

public class ChangeBusStatusService : IChangeBusStatusUseCase
{
    private readonly IBusRepository _busRepository;
    private readonly BusAccessPolicy _accessPolicy;

    public ChangeBusStatusService(IBusRepository busRepository,
        BusAccessPolicy accessPolicy)
    {
        _busRepository = busRepository;
        _accessPolicy = accessPolicy;
    }

    public async Task ExecuteAsync(Guid busId, ChangeStatusRequest request, CancellationToken ct = default)
    {
        if (!Enum.TryParse<Status>(request.Status, true, out var status))
            throw new ArgumentException($"Invalid status: {request.Status}");

        var bus = await _busRepository.GetByIdAsync(busId, ct)
            ?? throw new InvalidOperationException("Bus not found");

        await _accessPolicy.EnsureCanManageBusAsync(bus, ct);

        bus.Status = status;
        await _busRepository.UpdateAsync(bus, ct);
    }
}