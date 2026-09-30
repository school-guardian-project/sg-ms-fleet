using ms_fleet.Api.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;
using ms_fleet.Api.Infrastructure.Persistence;
using ms_fleet.Api.Infrastructure.Persistence.Context;

namespace ms_fleet.Api.Infrastructure.Persistence.Repository;

public class DriverAssignmentRepository : IDriverAssignmentRepository
{
    private readonly FleetContext _context;

    public DriverAssignmentRepository(FleetContext context)
    {
        _context = context;
    }

    public async Task<DriverAssignment?> GetActiveByBusIdAsync(Guid busId, CancellationToken ct = default)
    {
        var entity = await _context.DriverAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusId == busId && x.AssignedTo == null, ct);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<DriverAssignment?> GetActiveByProfileIdAsync(Guid profileId, CancellationToken ct = default)
    {
        var entity = await _context.DriverAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProfileId == profileId && x.AssignedTo == null, ct);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task AddAsync(DriverAssignment assignment, CancellationToken ct = default)
    {
        var entity = new DriverAssignmentEntity
        {
            Id = assignment.Id,
            ProfileId = assignment.ProfileId,
            BusId = assignment.BusId,
            AssignedFrom = assignment.AssignedFrom
        };
        await _context.DriverAssignments.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task CloseActiveAsync(Guid busId, CancellationToken ct = default)
    {
        var entity = await _context.DriverAssignments
            .FirstOrDefaultAsync(x => x.BusId == busId && x.AssignedTo == null, ct)
            ?? throw new InvalidOperationException("No active assignment found");

        entity.AssignedTo = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
    }

    private static DriverAssignment ToDomain(DriverAssignmentEntity entity) => new()
    {
        Id = entity.Id,
        ProfileId = entity.ProfileId,
        BusId = entity.BusId,
        AssignedFrom = entity.AssignedFrom,
        AssignedTo = entity.AssignedTo
    };
}
