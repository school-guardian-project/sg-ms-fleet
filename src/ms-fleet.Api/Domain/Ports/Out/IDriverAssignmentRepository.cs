namespace ms_fleet.Api.Domain.Ports.Out;

public interface IDriverAssignmentRepository
{
    Task<DriverAssignment?> GetActiveByBusIdAsync(Guid busId, CancellationToken ct = default);
    Task<DriverAssignment?> GetActiveByProfileIdAsync(Guid profileId, CancellationToken ct = default);
    Task AddAsync(DriverAssignment assignment, CancellationToken ct = default);
    Task CloseActiveAsync(Guid busId, CancellationToken ct = default);
}
