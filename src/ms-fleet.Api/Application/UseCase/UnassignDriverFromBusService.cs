using ms_fleet.Api.Application.Security;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class UnassignDriverFromBusService : IUnassignDriverFromBusUseCase
{
    private readonly IDriverAssignmentRepository _assignmentRepository;
    private readonly IBusRepository _busRepository;
    private readonly BusAccessPolicy _accessPolicy;

    public UnassignDriverFromBusService(
        IDriverAssignmentRepository assignmentRepository,
        IBusRepository busRepository,
        BusAccessPolicy accessPolicy)
    {
        _assignmentRepository = assignmentRepository;
        _busRepository = busRepository;
        _accessPolicy = accessPolicy;
    }

    public async Task ExecuteAsync(Guid busId, CancellationToken ct = default)
    {
        var bus = await _busRepository.GetByIdAsync(busId, ct)
            ?? throw new InvalidOperationException("Bus not found");

        await _accessPolicy.EnsureCanManageBusAsync(bus, ct);

        _ = await _assignmentRepository.GetActiveByBusIdAsync(busId, ct)
            ?? throw new InvalidOperationException("No active driver assignment for this bus");

        await _assignmentRepository.CloseActiveAsync(busId, ct);
    }
}
