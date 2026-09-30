namespace ms_fleet.Api.Domain.Model;

public class Brand
{
    public byte Id { get; set; }
    public string? Name { get; set; }
    public Status Status { get; set; } = Status.Active;
}
