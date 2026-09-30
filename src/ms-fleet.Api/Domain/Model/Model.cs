using ms_fleet.Api.Domain.Model;
namespace ms_fleet.Api.Domain.Model;

public class Model
{
    public byte Id { get; set; }
    public byte BrandId { get; set; }
    public string? Name { get; set; }
    public Status Status { get; set; } = Status.Active;
}
