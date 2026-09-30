using ms_fleet.Api.Domain.Ports.Out;
using Xunit;
using Xunit;
using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Tests.Fakes;

public class InMemoryDriverAssignmentRepository : IDriverAssignmentRepository
{
    private readonly List<DriverAssignment> _assignments = new();

    public Task<DriverAssignment?> GetActiveByBusIdAsync(Guid busId, CancellationToken ct = default)
        => Task.FromResult(_assignments.FirstOrDefault(x => x.BusId == busId && x.AssignedTo == null));

    public Task<DriverAssignment?> GetActiveByProfileIdAsync(Guid profileId, CancellationToken ct = default)
        => Task.FromResult(_assignments.FirstOrDefault(x => x.ProfileId == profileId && x.AssignedTo == null));

    public Task AddAsync(DriverAssignment assignment, CancellationToken ct = default)
    {
        _assignments.Add(assignment);
        return Task.CompletedTask;
    }

    public Task CloseActiveAsync(Guid busId, CancellationToken ct = default)
    {
        var assignment = _assignments.FirstOrDefault(x => x.BusId == busId && x.AssignedTo == null);
        if (assignment != null) assignment.AssignedTo = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}
