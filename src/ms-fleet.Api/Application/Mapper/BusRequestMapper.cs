using ms_fleet.Api.Application.Dto.Request;
using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Application.Mapper;

/// <summary>
/// Maps Request DTOs to Bus domain entities.
/// This mapper is stateless and can be registered as a singleton.
/// </summary>
public interface IBusRequestMapper
{
    Bus ToDomain(CreateBusRequest request);
    void UpdateDomain(Bus bus, UpdateBusRequest request);
}

public sealed class BusRequestMapper : IBusRequestMapper
{
    public Bus ToDomain(CreateBusRequest request)
        => new Bus
        {
            CampuseId = request.CampuseId,
            SoatValidity = request.SoatValidity,
            GpsDeviceId = request.GpsDeviceId,
            Capacity = request.Capacity,
            Plate = request.Plate.Trim(),
            ModelId = (byte)request.ModelId,
            Status = Status.Active
        };

    public void UpdateDomain(Bus bus, UpdateBusRequest request)
    {
        if (request.CampuseId.HasValue && request.CampuseId != Guid.Empty)
            bus.CampuseId = request.CampuseId.Value;

        if (request.SoatValidity.HasValue && request.SoatValidity != default)
            bus.SoatValidity = request.SoatValidity.Value;

        if (request.ModelId.HasValue && request.ModelId > 0)
            bus.ModelId = (byte)request.ModelId.Value;

        if (!string.IsNullOrEmpty(request.Plate))
            bus.Plate = request.Plate.Trim();

        if (request.Capacity.HasValue)
            bus.Capacity = request.Capacity.Value;

        if (request.GpsDeviceId.HasValue && request.GpsDeviceId != Guid.Empty)
            bus.GpsDeviceId = request.GpsDeviceId.Value;
    }
}