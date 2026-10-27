namespace ms_fleet.Api.Application.Dto.Response;

/// <summary>
/// Response DTO for bus list item (minimal info for listings).
/// </summary>
/// <param name="Id">Bus identifier.</param>
/// <param name="Plate">License plate.</param>
/// <param name="CampuseId">Campus identifier.</param>
/// <param name="DriverName">Assigned driver name.</param>
/// <param name="Brand">Vehicle brand name.</param>
/// <param name="Model">Vehicle model name.</param>
/// <param name="Status">Bus status as string.</param>
public sealed record BusListItemResponse(
    Guid Id,
    string Plate,
    Guid CampuseId,
    string DriverName,
    string Brand,
    string Model,
    string Status);