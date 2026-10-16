namespace ms_fleet.Api.Infrastructure.Persistence.Entity;

/// <summary>
/// Referencia a Gps.GpsDevice. ms-fleet solo lee el dispositivo y cambia su
/// estado (activo/inactivo) cuando se edita el bus que lo lleva.
/// </summary>
public class GpsDeviceEntity
{
    public Guid Id { get; set; }
    public string Imei { get; set; } = string.Empty;
    public DateTime LastConnection { get; set; }
    public bool GpsStatus { get; set; }
}
