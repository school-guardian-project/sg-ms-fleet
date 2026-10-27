namespace ms_fleet.Api.Application.Dto.Response;

/// <summary>
/// Response DTO for model list item.
/// </summary>
/// <param name="Id">Model identifier.</param>
/// <param name="Name">Model name.</param>
/// <param name="BrandId">Brand identifier.</param>
/// <param name="BrandName">Brand name.</param>
public sealed record ModelListResponse(
    byte Id,
    string Name,
    byte BrandId,
    string BrandName);