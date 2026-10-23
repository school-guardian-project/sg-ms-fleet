namespace ms_fleet.Api.Application.Dtos;

public class CreateBusRequest
{
    public Guid CampuseId { get; set; }
    public DateTime SoatValidity { get; set; }
    public Guid GpsDeviceId { get; set; }
    public byte Capacity { get; set; }
    public string Plate { get; set; } = string.Empty;
    public int ModelId { get; set; }
    public bool? GpsStatus { get; set; }
    public string? GpsImei { get; set; }
}
