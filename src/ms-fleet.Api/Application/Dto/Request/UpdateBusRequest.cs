using System.ComponentModel.DataAnnotations;

namespace ms_fleet.Api.Application.Dto.Request;

/// <summary>
/// Request DTO for updating an existing bus.
/// All properties are optional - only provided values will be updated.
/// </summary>
public sealed class UpdateBusRequest
{
    /// <summary>
    /// Campus identifier where the bus operates.
    /// </summary>
    public Guid? CampuseId { get; init; }

    /// <summary>
    /// SOAT validity expiration date.
    /// </summary>
    public DateTime? SoatValidity { get; init; }

    /// <summary>
    /// Passenger capacity of the bus.
    /// </summary>
    [Range(1, byte.MaxValue)]
    public byte? Capacity { get; init; }

    /// <summary>
    /// License plate of the bus.
    /// </summary>
    [StringLength(20, MinimumLength = 1)]
    public string? Plate { get; init; }

    /// <summary>
    /// Vehicle model identifier.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int? ModelId { get; init; }

    /// <summary>
    /// GPS device identifier to associate with the bus.
    /// </summary>
    public Guid? GpsDeviceId { get; init; }

    /// <summary>
    /// Optional GPS device status override.
    /// </summary>
    public bool? GpsStatus { get; init; }

    /// <summary>
    /// Optional GPS device IMEI for resolution when GpsDeviceId is not provided.
    /// </summary>
    public string? GpsImei { get; init; }
}