namespace ms_fleet.Api.Application.Dto.Response;

/// <summary>
/// Response DTO for brand list item.
/// </summary>
/// <param name="Id">Brand identifier.</param>
/// <param name="Name">Brand name.</param>
public sealed record BrandListResponse(
    byte Id,
    string Name);