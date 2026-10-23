using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Infrastructure.Persistence.Entity;

public class BrandEntity
{
    public byte Id { get; set; }
    public string? Name { get; set; }
    public Status Status { get; set; } = Status.Active;
}
