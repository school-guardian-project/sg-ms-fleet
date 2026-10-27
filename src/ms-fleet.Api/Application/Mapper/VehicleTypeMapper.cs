using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Domain.Model;

namespace ms_fleet.Api.Application.Mapper;

/// <summary>
/// Maps Brand and Model domain entities to Response DTOs.
/// This mapper is stateless and can be registered as a singleton.
/// </summary>
public interface IVehicleTypeMapper
{
    BrandListResponse ToBrandList(Brand brand);
    ModelListResponse ToModelList(Model model, string? brandName);
}

public sealed class VehicleTypeMapper : IVehicleTypeMapper
{
    public BrandListResponse ToBrandList(Brand brand)
        => new(brand.Id, brand.Name ?? string.Empty);

    public ModelListResponse ToModelList(Model model, string? brandName)
        => new(model.Id, model.Name ?? string.Empty, model.BrandId, brandName ?? string.Empty);
}