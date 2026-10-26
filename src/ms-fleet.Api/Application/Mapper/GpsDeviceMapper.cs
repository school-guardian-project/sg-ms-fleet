using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Application.Mapper;

/// <summary>
/// Maps GPS device domain entities to Response DTOs.
/// This mapper is stateless and can be registered as a singleton.
/// </summary>
public interface IGpsDeviceMapper
{
    GpsDeviceResponse ToResponse(GpsDeviceInfo device);
    IEnumerable<GpsDeviceResponse> ToResponses(IEnumerable<GpsDeviceInfo> devices);
}

public sealed class GpsDeviceMapper : IGpsDeviceMapper
{
    public GpsDeviceResponse ToResponse(GpsDeviceInfo device)
        => new(device.Id, device.Imei, device.GpsStatus, device.AssignedBusId, device.AssignedPlate);

    public IEnumerable<GpsDeviceResponse> ToResponses(IEnumerable<GpsDeviceInfo> devices)
        => devices.Select(ToResponse);
}