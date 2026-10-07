using Microsoft.EntityFrameworkCore;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;
using ms_fleet.Api.Infrastructure.Persistence;
using ms_fleet.Api.Infrastructure.Persistence.Context;

namespace ms_fleet.Api.Infrastructure.Persistence.Repository;

public class BrandRepositoryImpl : IBrandRepository
{
    private readonly FleetContext _context;

    public BrandRepositoryImpl(FleetContext context)
    {
        _context = context;
    }

    public async Task<List<Brand>> ListActiveAsync(CancellationToken ct)
    {
        return await _context.Brands
            .AsNoTracking()
            .Where(b => b.Status == Status.Active)
            .OrderBy(b => b.Name)
            .Select(b => new Brand
            {
                Id = b.Id,
                Name = b.Name,
                Status = b.Status
            })
            .ToListAsync(ct);
    }
}
