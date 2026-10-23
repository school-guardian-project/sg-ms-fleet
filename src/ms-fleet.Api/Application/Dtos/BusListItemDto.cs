namespace ms_fleet.Api.Application.Dtos;

public record BusListItemDto(
    Guid Id,
    string Plate,
    Guid CampuseId,
    string DriverName,
    string Brand,
    string Model,
    string Status);
