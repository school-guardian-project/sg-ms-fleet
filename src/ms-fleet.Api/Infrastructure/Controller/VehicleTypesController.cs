using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ms_fleet.Api.Application.Dtos;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Infrastructure.Persistence.Context;

namespace ms_fleet.Api.Infrastructure.Controller;

[ApiController]
[Route("api/vehicle-types")]
public class VehicleTypesController : ControllerBase
{
    private readonly FleetContext _context;

    public VehicleTypesController(FleetContext context)
    {
        _context = context;
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<BrandListDto>>> ListBrands(CancellationToken ct)
    {
        var brands = await _context.Brands
            .AsNoTracking()
            .Where(brand => brand.Status == Status.Active)
            .OrderBy(brand => brand.Name)
            .Select(brand => new BrandListDto(brand.Id, brand.Name ?? string.Empty))
            .ToListAsync(ct);

        return Ok(brands);
    }

    [HttpGet("models")]
    public async Task<ActionResult<IReadOnlyList<ModelListDto>>> ListModels(
        [FromQuery] byte? brandId,
        CancellationToken ct)
    {
        var models = await (
                from model in _context.Models.AsNoTracking()
                join brand in _context.Brands.AsNoTracking() on model.BrandId equals brand.Id
                where model.Status == Status.Active
                    && brand.Status == Status.Active
                    && (brandId == null || model.BrandId == brandId.Value)
                orderby model.Name
                select new ModelListDto(
                    model.Id,
                    model.Name ?? string.Empty,
                    brand.Id,
                    brand.Name ?? string.Empty))
            .ToListAsync(ct);

        return Ok(models);
    }
}
