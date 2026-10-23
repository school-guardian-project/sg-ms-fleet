using ms_fleet.Api.Application.Dtos;

namespace ms_fleet.Api.Application.Mapper;

/// <summary>
/// Mapeos de dispositivos GPS a DTO de respuesta.
/// </summary>
public static class GpsDeviceMapper
{
    public static GpsDeviceDto ToDto(GpsDeviceInfo device)
        => new(device.Id, device.Imei, device.GpsStatus, device.AssignedBusId, device.AssignedPlate);

    public static IEnumerable<GpsDeviceDto> ToDtos(IEnumerable<GpsDeviceInfo> devices)
        => devices.Select(ToDto);
}
