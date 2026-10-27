using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Application.Mapper;

/// <summary>
/// Maps Bus domain entities to Response DTOs.
/// This mapper is stateless and can be registered as a singleton.
/// </summary>
public interface IBusMapper
{
    BusListItemResponse ToListItem(Bus bus, string driverName, ModelNames? names);
    BusDetailResponse ToDetail(Bus bus, string driverName, ModelNames? names, GpsDeviceInfo? gps, Guid? driverProfileId);
    BusAssignedResponse ToAssigned(Bus bus);
}

public sealed class BusMapper : IBusMapper
{
    public BusListItemResponse ToListItem(Bus bus, string driverName, ModelNames? names)
        => new(
            bus.Id,
            bus.Plate,
            bus.CampuseId,
            driverName,
            names?.Brand ?? string.Empty,
            names?.Model ?? string.Empty,
            bus.Status.ToString());

    public BusDetailResponse ToDetail(
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

    public BusAssignedResponse ToAssigned(Bus bus)
        => new(bus.Id, bus.Plate, bus.CampuseId);
}