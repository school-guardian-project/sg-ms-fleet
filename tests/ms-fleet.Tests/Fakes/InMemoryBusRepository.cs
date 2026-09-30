using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Tests.Fakes;

public class InMemoryBusRepository : IBusRepository
{
    private readonly List<Bus> _buses = new();

    public Task<Bus?> GetByIdAsync(Guid busId, CancellationToken ct = default)
        => Task.FromResult(_buses.FirstOrDefault(x => x.Id == busId));

    public Task<IReadOnlyList<Bus>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Bus>>(_buses.ToList());

    public Task<bool> ExistsByPlateAsync(string plate, CancellationToken ct = default)
        => Task.FromResult(_buses.Any(x => x.Plate == plate));

    public Task AddAsync(Bus bus, CancellationToken ct = default)
    {
        _buses.Add(bus);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Bus bus, CancellationToken ct = default)
    {
        var index = _buses.FindIndex(x => x.Id == bus.Id);
        if (index >= 0) _buses[index] = bus;
        return Task.CompletedTask;
    }
}
