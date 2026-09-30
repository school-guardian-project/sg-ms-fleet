namespace ms_fleet.Api.Infrastructure.Persistence.Entity;

public class DriverAssignmentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProfileId { get; set; }
    public Guid BusId { get; set; }
    public DateTime AssignedFrom { get; set; } = DateTime.UtcNow;
    public DateTime? AssignedTo { get; set; }
}
