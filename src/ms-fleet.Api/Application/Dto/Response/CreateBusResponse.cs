namespace ms_fleet.Api.Application.Dto.Response;

/// <summary>
/// Response DTO for bus creation operation.
/// </summary>
/// <param name="BusId">Created bus identifier.</param>
public sealed record CreateBusResponse(
    Guid BusId);