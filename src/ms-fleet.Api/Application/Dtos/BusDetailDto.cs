namespace ms_fleet.Api.Application.Dtos;

public record BusDetailDto(
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
