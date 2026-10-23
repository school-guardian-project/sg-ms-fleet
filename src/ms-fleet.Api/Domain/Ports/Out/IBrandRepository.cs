using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Domain.Ports.Out;

public interface IBrandRepository
{
    Task<List<Brand>> ListActiveAsync(CancellationToken ct);
}
