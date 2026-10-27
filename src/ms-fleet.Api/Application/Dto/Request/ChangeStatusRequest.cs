using System.ComponentModel.DataAnnotations;

namespace ms_fleet.Api.Application.Dto.Request;

/// <summary>
/// Request DTO for changing bus status.
/// </summary>
public sealed class ChangeStatusRequest
{
    /// <summary>
    /// New status value (Active, Inactive, Maintenance).
    /// </summary>
    [Required]
    [StringLength(20, MinimumLength = 1)]
    public string Status { get; init; } = string.Empty;
}