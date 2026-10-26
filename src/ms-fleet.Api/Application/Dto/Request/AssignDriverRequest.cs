using System.ComponentModel.DataAnnotations;

namespace ms_fleet.Api.Application.Dto.Request;

/// <summary>
/// Request DTO for assigning a driver to a bus.
/// </summary>
public sealed class AssignDriverRequest
{
    /// <summary>
    /// Driver profile identifier.
    /// </summary>
    [Required]
    public Guid ProfileId { get; init; }
}