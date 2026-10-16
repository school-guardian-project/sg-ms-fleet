using ms_fleet.Api.Domain.Model;
namespace ms_fleet.Api.Domain.Ports.Out;

public interface IBusRepository
{
    Task<Bus?> GetByIdAsync(Guid busId, CancellationToken ct = default);
    /// <param name="campusIds">Sedes permitidas; null devuelve todos los buses (sin filtro de tenant).</param>
    Task<IReadOnlyList<Bus>> GetAllAsync(IReadOnlyCollection<Guid>? campusIds = null, CancellationToken ct = default);
    Task<bool> ExistsByPlateAsync(string plate, CancellationToken ct = default);
    Task<ModelNames?> GetModelNamesAsync(byte modelId, CancellationToken ct = default);
    Task AddAsync(Bus bus, CancellationToken ct = default);
    Task UpdateAsync(Bus bus, CancellationToken ct = default);
}
