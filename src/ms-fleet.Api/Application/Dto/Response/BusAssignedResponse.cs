namespace ms_fleet.Api.Application.Dto.Response;

/// <summary>
/// Response DTO for bus assigned to driver (minimal info).
/// </summary>
/// <param name="Id">Bus identifier.</param>
/// <param name="Plate">License plate.</param>
/// <param name="CampuseId">Campus identifier.</param>
public sealed record BusAssignedResponse(
    Guid Id,
    string Plate,
    Guid CampuseId);