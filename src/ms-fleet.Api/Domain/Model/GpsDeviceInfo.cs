namespace ms_fleet.Api.Domain.Model;

/// <summary>
/// Información de un dispositivo GPS.
/// </summary>
public sealed record GpsDeviceInfo(
    Guid Id,
    string Imei,
    bool GpsStatus,
    Guid? AssignedBusId,
    string? AssignedPlate);