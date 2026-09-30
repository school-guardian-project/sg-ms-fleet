using ms_fleet.Api.Domain.Model;
namespace ms_fleet.Api.Domain.Ports.Out;

public interface IBusRepository
{
    Task<Bus?> GetByIdAsync(Guid busId, CancellationToken ct = default);
    Task<IReadOnlyList<Bus>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsByPlateAsync(string plate, CancellationToken ct = default);
    Task AddAsync(Bus bus, CancellationToken ct = default);
    Task UpdateAsync(Bus bus, CancellationToken ct = default);
}
