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

    public async Task<IReadOnlyList<Bus>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _context.Buses.AsNoTracking().ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<bool> ExistsByPlateAsync(string plate, CancellationToken ct = default)
    {
        return await _context.Buses.AnyAsync(x => x.Plate == plate, ct);
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
        ModelId = entity.ModelId,
        Status = entity.Status
    };
}
