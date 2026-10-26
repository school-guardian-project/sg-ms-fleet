namespace ms_fleet.Api.Application.Dto.Request;

/// <summary>
/// Request DTO for searching buses.
/// </summary>
public sealed class SearchBusesRequest
{
    /// <summary>
    /// Search term (plate, brand, model, etc.).
    /// </summary>
    public string? SearchTerm { get; init; }
}