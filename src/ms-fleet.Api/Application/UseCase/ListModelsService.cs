using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class ListModelsService : IListModelsUseCase
{
    private readonly IModelRepository _modelRepository;
    private readonly IVehicleTypeMapper _vehicleTypeMapper;

    public ListModelsService(IModelRepository modelRepository, IVehicleTypeMapper vehicleTypeMapper)
    {
        _modelRepository = modelRepository;
        _vehicleTypeMapper = vehicleTypeMapper;
    }

    public async Task<List<ModelListResponse>> ExecuteAsync(byte? brandId, CancellationToken ct)
    {
        var models = await _modelRepository.ListActiveAsync(brandId, ct);

        var result = new List<ModelListResponse>();
        foreach (var model in models)
        {
            var brandName = await _modelRepository.GetBrandNameAsync(model.BrandId, ct);
            result.Add(_vehicleTypeMapper.ToModelList(model, brandName));
        }

        return result;
    }
}