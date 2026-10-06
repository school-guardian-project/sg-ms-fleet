using ms_fleet.Api.Domain.Model;
namespace ms_fleet.Api.Domain.Model;

public class Bus
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampuseId { get; set; }
    public DateTime SoatValidity { get; set; }
    public Guid GpsDeviceId { get; set; }
    public byte Capacity { get; set; }
    public string Plate { get; set; } = string.Empty;
    public byte ModelId { get; set; }
    public Status Status { get; set; } = Status.Active;
}
