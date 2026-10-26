namespace ms_fleet.Api.Application.Dto.Response;

/// <summary>
/// Response DTO for GPS device information.
/// </summary>
/// <param name="Id">GPS device identifier.</param>
/// <param name="Imei">Device IMEI.</param>
/// <param name="GpsStatus">Device status.</param>
/// <param name="AssignedBusId">Assigned bus identifier.</param>
/// <param name="AssignedPlate">Assigned bus plate.</param>
public sealed record GpsDeviceResponse(
    Guid Id,
    string Imei,
    bool GpsStatus,
    Guid? AssignedBusId,
    string? AssignedPlate);