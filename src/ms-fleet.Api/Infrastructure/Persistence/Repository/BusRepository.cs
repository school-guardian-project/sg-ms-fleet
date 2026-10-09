using ms_fleet.Api.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;
using ms_fleet.Api.Infrastructure.Persistence;
using ms_fleet.Api.Infrastructure.Persistence.Context;

namespace ms_fleet.Api.Infrastructure.Persistence.Repository;

public class BusRepository : IBusRepository
{
    private readonly FleetContext _context;

    public BusRepository(FleetContext context)
    {
        _context = context;
    }

    public async Task<Bus?> GetByIdAsync(Guid busId, CancellationToken ct = default)
    {
        var entity = await _context.Buses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == busId, ct);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Bus>> GetAllAsync(IReadOnlyCollection<Guid>? campusIds = null, CancellationToken ct = default)
    {
        var query = _context.Buses.AsNoTracking().Where(x => x.Status == Status.Active);

        if (campusIds is not null)
            query = query.Where(x => campusIds.Contains(x.CampuseId));

        var entities = await query.ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<bool> ExistsByPlateAsync(string plate, CancellationToken ct = default)
    {
        return await _context.Buses.AnyAsync(x => x.Plate == plate, ct);
    }

    public async Task<ModelNames?> GetModelNamesAsync(byte modelId, CancellationToken ct = default)
    {
        var row = await (
                from m in _context.Models.AsNoTracking()
                join b in _context.Brands.AsNoTracking() on m.BrandId equals b.Id
                where m.Id == modelId
                select new { Brand = b.Name, Model = m.Name })
            .FirstOrDefaultAsync(ct);

        return row is null ? null : new ModelNames(row.Brand, row.Model);
    }

    public async Task AddAsync(Bus bus, CancellationToken ct = default)
    {
        var entity = new BusEntity
        {
            Id = bus.Id,
            CampuseId = bus.CampuseId,
            SoatValidity = bus.SoatValidity,
            GpsDeviceId = bus.GpsDeviceId,
            Capacity = bus.Capacity,
            Plate = bus.Plate,
            ModelId = bus.ModelId,
            Status = bus.Status
        };
        await _context.Buses.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Bus bus, CancellationToken ct = default)
    {
        var entity = await _context.Buses.FirstOrDefaultAsync(x => x.Id == bus.Id, ct)
            ?? throw new InvalidOperationException("Bus not found");

        entity.CampuseId = bus.CampuseId;
        entity.SoatValidity = bus.SoatValidity;
        entity.Capacity = bus.Capacity;
        entity.ModelId = bus.ModelId;
        entity.Plate = bus.Plate;
        entity.GpsDeviceId = bus.GpsDeviceId;
        entity.Status = bus.Status;

        await _context.SaveChangesAsync(ct);
    }

    private static Bus ToDomain(BusEntity entity) => new()
    {
        Id = entity.Id,
        CampuseId = entity.CampuseId,
        SoatValidity = entity.SoatValidity,
        GpsDeviceId = entity.GpsDeviceId,
        Capacity = entity.Capacity,
        Plate = entity.Plate,
        ModelId = (byte)entity.ModelId,
        Status = entity.Status
    };
}
