using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Infrastructure.Persistence.Entity;

public class BusEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampuseId { get; set; }
    public DateTime SoatValidity { get; set; }
    public Guid GpsDeviceId { get; set; }
    public byte Capacity { get; set; }
    public string Plate { get; set; } = string.Empty;
    public int ModelId { get; set; }
    public Status Status { get; set; } = Status.Active;
}
