using ms_fleet.Api.Infrastructure.Auth;
using ms_fleet.Api.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Domain.Ports.In;

namespace ms_fleet.Api.Infrastructure.Controller;

[ApiController]
[Route("api/vehicle-types")]
public class VehicleTypeController : ControllerBase
{
    private readonly IListBrandsUseCase _listBrandsUseCase;
    private readonly IListModelsUseCase _listModelsUseCase;

    public VehicleTypeController(
        IListBrandsUseCase listBrandsUseCase,
        IListModelsUseCase listModelsUseCase)
    {
        _listBrandsUseCase = listBrandsUseCase;
        _listModelsUseCase = listModelsUseCase;
    }

    [RequirePermission(FleetPermissions.BusesRead)]
    [HttpGet("brands")]
    public async Task<ActionResult<List<BrandListResponse>>> ListBrands(CancellationToken ct)
    {
        var brands = await _listBrandsUseCase.ExecuteAsync(ct);
        return Ok(brands);
    }

    [RequirePermission(FleetPermissions.BusesRead)]
    [HttpGet("models")]
    public async Task<ActionResult<List<ModelListResponse>>> ListModels([FromQuery] byte? brandId, CancellationToken ct)
    {
        var models = await _listModelsUseCase.ExecuteAsync(brandId, ct);
        return Ok(models);
    }
}