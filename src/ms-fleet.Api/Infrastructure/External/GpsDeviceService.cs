using Microsoft.EntityFrameworkCore;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;
using ms_fleet.Api.Infrastructure.Persistence.Context;
using ms_fleet.Api.Infrastructure.Persistence.Entity;

namespace ms_fleet.Api.Infrastructure.External;

/// <summary>
/// Dispositivos GPS leidos directamente de Gps.GpsDevice (misma base de datos).
/// Antes se consultaba un endpoint HTTP que no existe, asi que crear un bus
/// fallaba siempre la validacion del GPS.
/// </summary>
public class GpsDeviceService : IGpsDeviceService
{
    private readonly FleetContext _context;

    public GpsDeviceService(FleetContext context)
    {
        _context = context;
    }

    public Task<bool> GpsDeviceExistsAsync(Guid gpsDeviceId, CancellationToken ct = default)
        => _context.GpsDevices.AsNoTracking().AnyAsync(g => g.Id == gpsDeviceId, ct);

    public async Task<GpsDeviceInfo?> GetAsync(Guid gpsDeviceId, CancellationToken ct = default)
        => (await QueryAsync(gpsDeviceId, ct)).FirstOrDefault();

    public Task<IReadOnlyList<GpsDeviceInfo>> ListAsync(CancellationToken ct = default)
        => QueryAsync(null, ct);

    public async Task SetStatusAsync(Guid gpsDeviceId, bool active, CancellationToken ct = default)
    {
        var device = await _context.GpsDevices.FirstOrDefaultAsync(g => g.Id == gpsDeviceId, ct)
            ?? throw new ArgumentException("GPS device not found");
        if (device.GpsStatus == active) return;
        device.GpsStatus = active;
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Guid> ResolveByImeiAsync(string imei, CancellationToken ct = default)
    {
        imei = imei?.Trim() ?? string.Empty;
        if (imei.Length != 15 || !imei.All(char.IsDigit))
            throw new ArgumentException("Invalid IMEI");

        var existing = await _context.GpsDevices.AsNoTracking().FirstOrDefaultAsync(g => g.Imei == imei, ct);
        if (existing is not null) return existing.Id;

        var device = new GpsDeviceEntity
        {
            Id = Guid.NewGuid(),
            Imei = imei,
            GpsStatus = true,
            LastConnection = DateTime.UtcNow
        };
        await _context.GpsDevices.AddAsync(device, ct);
        await _context.SaveChangesAsync(ct);
        return device.Id;
    }

    private async Task<IReadOnlyList<GpsDeviceInfo>> QueryAsync(Guid? id, CancellationToken ct)
    {
        var devices = _context.GpsDevices.AsNoTracking();
        if (id is not null) devices = devices.Where(g => g.Id == id);

        var list = await devices.OrderBy(g => g.Imei).ToListAsync(ct);
        var ids = list.Select(g => g.Id).ToList();

        // Fleet.Bus tiene UNIQUE sobre GpsDeviceId, asi que tambien un bus
        // inactivo deja el dispositivo ocupado.
        var buses = await _context.Buses.AsNoTracking()
            .Where(b => ids.Contains(b.GpsDeviceId))
            .Select(b => new { b.Id, b.Plate, b.GpsDeviceId })
            .ToListAsync(ct);
        var busByGps = buses.GroupBy(b => b.GpsDeviceId).ToDictionary(g => g.Key, g => g.First());

        return list.Select(g =>
        {
            busByGps.TryGetValue(g.Id, out var bus);
            return new GpsDeviceInfo(g.Id, g.Imei, g.GpsStatus, bus?.Id, bus?.Plate ?? string.Empty);
        }).ToList();
    }
}
