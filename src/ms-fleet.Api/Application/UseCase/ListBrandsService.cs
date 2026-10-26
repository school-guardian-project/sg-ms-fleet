using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class ListBrandsService : IListBrandsUseCase
{
    private readonly IBrandRepository _brandRepository;
    private readonly IVehicleTypeMapper _vehicleTypeMapper;

    public ListBrandsService(IBrandRepository brandRepository, IVehicleTypeMapper vehicleTypeMapper)
    {
        _brandRepository = brandRepository;
        _vehicleTypeMapper = vehicleTypeMapper;
    }

    public async Task<List<BrandListResponse>> ExecuteAsync(CancellationToken ct)
    {
        var brands = await _brandRepository.ListActiveAsync(ct);
        return brands.Select(_vehicleTypeMapper.ToBrandList).ToList();
    }
}