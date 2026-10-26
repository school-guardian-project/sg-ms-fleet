using System.ComponentModel.DataAnnotations;

namespace ms_fleet.Api.Application.Dto.Request;

/// <summary>
/// Request DTO for creating a new bus.
/// </summary>
public sealed class CreateBusRequest
{
    /// <summary>
    /// Campus identifier where the bus operates.
    /// </summary>
    [Required]
    public Guid CampuseId { get; init; }

    /// <summary>
    /// SOAT validity expiration date.
    /// </summary>
    [Required]
    public DateTime SoatValidity { get; init; }

    /// <summary>
    /// GPS device identifier to associate with the bus.
    /// </summary>
    [Required]
    public Guid GpsDeviceId { get; init; }

    /// <summary>
    /// Passenger capacity of the bus.
    /// </summary>
    [Required]
    [Range(1, byte.MaxValue)]
    public byte Capacity { get; init; }

    /// <summary>
    /// License plate of the bus.
    /// </summary>
    [Required]
    [StringLength(20, MinimumLength = 1)]
    public string Plate { get; init; } = string.Empty;

    /// <summary>
    /// Vehicle model identifier.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int ModelId { get; init; }

    /// <summary>
    /// Optional GPS device status override.
    /// </summary>
    public bool? GpsStatus { get; init; }

    /// <summary>
    /// Optional GPS device IMEI for resolution when GpsDeviceId is not provided.
    /// </summary>
    public string? GpsImei { get; init; }
}