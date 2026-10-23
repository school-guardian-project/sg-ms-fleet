using ms_fleet.Api.Application.Dtos;
using ms_fleet.Api.Domain.Model;

// `Model` colisiona con el namespace `ms_fleet.Api.Domain.Model`: sin alias el
// compilador resuelve el identificador al namespace, no a la clase. El alias debe
// llevar otro nombre (igual que `RouteModel` en ms-route).
using VehicleModel = ms_fleet.Api.Domain.Model.Model;

namespace ms_fleet.Api.Application.Mapper;

/// <summary>
/// Mapeos de dominio a DTO de marcas y modelos de vehiculo.
/// </summary>
public static class VehicleTypeMapper
{
    public static BrandListDto ToBrandList(Brand brand)
        => new(brand.Id, brand.Name ?? "");

    public static ModelListDto ToModelList(VehicleModel model, string? brandName)
        => new(model.Id, model.Name ?? "", model.BrandId, brandName ?? "");
}
