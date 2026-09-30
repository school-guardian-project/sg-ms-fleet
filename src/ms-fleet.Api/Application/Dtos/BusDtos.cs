using ms_fleet.Api.Domain.Model;
namespace ms_fleet.Api.Application.Dtos;

public record BusListItemDto(
    Guid Id,
    string Plate,
    string DriverName,
    string Brand,
    string Model);

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
    string Model);
