namespace ms_fleet.Api.Domain.Ports.In;

public interface IListBrandsUseCase
{
    Task<List<Application.Dtos.BrandListDto>> ExecuteAsync(CancellationToken ct);
}

public interface IListModelsUseCase
{
    Task<List<Application.Dtos.ModelListDto>> ExecuteAsync(byte? brandId, CancellationToken ct);
}
