using ms_fleet.Api.Domain.Model;

// `Model` colisiona con el namespace `ms_fleet.Api.Domain.Model`: sin alias el
// compilador resuelve el identificador al namespace, no a la clase. El alias debe
// llevar otro nombre (igual que `RouteModel` en ms-route).
using VehicleModel = ms_fleet.Api.Domain.Model.Model;

namespace ms_fleet.Api.Domain.Ports.Out;

public interface IBrandRepository
{
    Task<List<Brand>> ListActiveAsync(CancellationToken ct);
}

public interface IModelRepository
{
    Task<List<VehicleModel>> ListActiveAsync(byte? brandId, CancellationToken ct);
    Task<string?> GetBrandNameAsync(byte brandId, CancellationToken ct);
}
