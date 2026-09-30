using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class UnassignDriverFromBusService : IUnassignDriverFromBusUseCase
{
    private readonly IDriverAssignmentRepository _assignmentRepository;

    public UnassignDriverFromBusService(IDriverAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task ExecuteAsync(Guid busId, CancellationToken ct = default)
    {
        var assignment = await _assignmentRepository.GetActiveByBusIdAsync(busId, ct)
            ?? throw new InvalidOperationException("No active driver assignment for this bus");

        await _assignmentRepository.CloseActiveAsync(busId, ct);
    }
}
