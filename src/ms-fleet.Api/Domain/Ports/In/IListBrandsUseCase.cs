using ms_fleet.Api.Application.Dto.Response;

namespace ms_fleet.Api.Domain.Ports.In;

public interface IListBrandsUseCase
{
    Task<List<BrandListResponse>> ExecuteAsync(CancellationToken ct);
}