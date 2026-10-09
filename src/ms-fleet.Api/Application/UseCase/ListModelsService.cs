using ms_fleet.Api.Application.Dtos;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class ListModelsService : IListModelsUseCase
{
    private readonly IModelRepository _modelRepository;

    public ListModelsService(IModelRepository modelRepository)
    {
        _modelRepository = modelRepository;
    }

    public async Task<List<ModelListDto>> ExecuteAsync(byte? brandId, CancellationToken ct)
    {
        var models = await _modelRepository.ListActiveAsync(brandId, ct);
        
        var result = new List<ModelListDto>();
        foreach (var model in models)
        {
            var brandName = await _modelRepository.GetBrandNameAsync(model.BrandId, ct);
            result.Add(new ModelListDto(model.Id, model.Name ?? "", model.BrandId, brandName ?? ""));
        }
        
        return result;
    }
}
