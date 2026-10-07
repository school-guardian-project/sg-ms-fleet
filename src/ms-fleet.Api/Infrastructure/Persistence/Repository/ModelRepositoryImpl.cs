using Microsoft.EntityFrameworkCore;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;
using ms_fleet.Api.Infrastructure.Persistence;
using ms_fleet.Api.Infrastructure.Persistence.Context;

// `Model` colisiona con el namespace `ms_fleet.Api.Domain.Model`: sin alias el
// compilador resuelve el identificador al namespace, no a la clase. El alias debe
// llevar otro nombre (igual que `RouteModel` en ms-route).
using VehicleModel = ms_fleet.Api.Domain.Model.Model;

namespace ms_fleet.Api.Infrastructure.Persistence.Repository;

public class ModelRepositoryImpl : IModelRepository
{
    private readonly FleetContext _context;

    public ModelRepositoryImpl(FleetContext context)
    {
        _context = context;
    }

    public async Task<List<VehicleModel>> ListActiveAsync(byte? brandId, CancellationToken ct)
    {
        var query = _context.Models
            .AsNoTracking()
            .Where(m => m.Status == Status.Active);

        if (brandId.HasValue)
        {
            query = query.Where(m => m.BrandId == brandId.Value);
        }

        return await query
            .OrderBy(m => m.Name)
            .Select(m => new VehicleModel
            {
                Id = m.Id,
                BrandId = m.BrandId,
                Name = m.Name,
                Status = m.Status
            })
            .ToListAsync(ct);
    }

    public async Task<string?> GetBrandNameAsync(byte brandId, CancellationToken ct)
    {
        return await _context.Brands
            .AsNoTracking()
            .Where(b => b.Id == brandId)
            .Select(b => b.Name)
            .FirstOrDefaultAsync(ct);
    }
}
