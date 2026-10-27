namespace ms_fleet.Api.Application.Dto.Response;

/// <summary>
/// Response DTO for bus detail (full information).
/// </summary>
/// <param name="Id">Bus identifier.</param>
/// <param name="Plate">License plate.</param>
/// <param name="CampuseId">Campus identifier.</param>
/// <param name="SoatValidity">SOAT validity expiration date.</param>
/// <param name="GpsDeviceId">GPS device identifier.</param>
/// <param name="Capacity">Passenger capacity.</param>
/// <param name="ModelId">Vehicle model identifier.</param>
/// <param name="Status">Bus status as string.</param>
/// <param name="DriverName">Assigned driver name.</param>
/// <param name="Brand">Vehicle brand name.</param>
/// <param name="Model">Vehicle model name.</param>
/// <param name="GpsImei">GPS device IMEI.</param>
/// <param name="GpsStatus">GPS device status.</param>
/// <param name="DriverProfileId">Assigned driver profile identifier.</param>
public sealed record BusDetailResponse(
    Guid Id,
    string Plate,
    Guid CampuseId,
    DateTime SoatValidity,
    Guid GpsDeviceId,
    byte Capacity,
    byte ModelId,
    string Status,
    string DriverName,
    string Brand,
    string Model,
    string GpsImei = "",
    bool? GpsStatus = null,
    Guid? DriverProfileId = null);