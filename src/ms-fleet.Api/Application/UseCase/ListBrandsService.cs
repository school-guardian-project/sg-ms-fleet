using ms_fleet.Api.Application.Dtos;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class ListBrandsService : IListBrandsUseCase
{
    private readonly IBrandRepository _brandRepository;

    public ListBrandsService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<List<BrandListDto>> ExecuteAsync(CancellationToken ct)
    {
        var brands = await _brandRepository.ListActiveAsync(ct);
        return brands.Select(b => new BrandListDto(b.Id, b.Name ?? "")).ToList();
    }
}
