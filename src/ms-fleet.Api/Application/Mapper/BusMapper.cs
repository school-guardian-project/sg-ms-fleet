using ms_fleet.Api.Application.Dtos;
using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Application.Mapper;

/// <summary>
/// Mapeos de dominio a DTO de las vistas de bus. La logica de resolucion
/// (asignaciones, nombres de modelo, cache) permanece en los servicios.
/// </summary>
public static class BusMapper
{
    public static BusListItemDto ToListItem(Bus bus, string driverName, ModelNames? names)
        => new(
            bus.Id,
            bus.Plate,
            bus.CampuseId,
            driverName,
            names?.Brand ?? string.Empty,
            names?.Model ?? string.Empty,
            bus.Status.ToString());

    public static BusDetailDto ToDetail(
        Bus bus,
        string driverName,
        ModelNames? names,
        GpsDeviceInfo? gps,
        Guid? driverProfileId)
        => new(
            bus.Id,
            bus.Plate,
            bus.CampuseId,
            bus.SoatValidity,
            bus.GpsDeviceId,
            bus.Capacity,
            bus.ModelId,
            bus.Status.ToString(),
            driverName,
            names?.Brand ?? string.Empty,
            names?.Model ?? string.Empty,
            gps?.Imei ?? string.Empty,
            gps?.GpsStatus,
            driverProfileId);

    public static BusAssignedDto ToAssigned(Bus bus)
        => new(bus.Id, bus.Plate, bus.CampuseId);
}
