using Microsoft.EntityFrameworkCore;
using ms_fleet.Api.Domain.Ports.Out;
using ms_fleet.Api.Infrastructure.Persistence.Context;

namespace ms_fleet.Api.Infrastructure.Persistence.Repository;

public class CampusReferenceRepository : ICampusReferenceRepository
{
    private readonly FleetContext _context;

    public CampusReferenceRepository(FleetContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Guid>> GetCampusIdsBySchoolAsync(Guid schoolId, CancellationToken ct = default)
    {
        return await _context.SchoolCampuses.AsNoTracking()
            .Where(x => x.SchoolId == schoolId)
            .Select(x => x.Id)
            .ToListAsync(ct);
    }
}
