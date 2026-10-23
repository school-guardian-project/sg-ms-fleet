namespace ms_fleet.Api.Domain.Ports.In;

public interface IListBrandsUseCase
{
    Task<List<Application.Dtos.BrandListDto>> ExecuteAsync(CancellationToken ct);
}
