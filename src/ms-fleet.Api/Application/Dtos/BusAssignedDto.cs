namespace ms_fleet.Api.Application.Dtos;

public record BusAssignedDto(
    Guid Id,
    string Plate,
    Guid CampuseId);
