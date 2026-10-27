using ms_fleet.Api.Infrastructure.Auth;
using ms_fleet.Api.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ms_fleet.Api.Application.Dto.Request;
using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Domain.Ports.In;

namespace ms_fleet.Api.Infrastructure.Controller;

[ApiController]
[Route("api/buses")]
public class BusController : ControllerBase
{
    private readonly ICreateBusUseCase _createBusUseCase;
    private readonly IUpdateBusUseCase _updateBusUseCase;
    private readonly IChangeBusStatusUseCase _changeBusStatusUseCase;
    private readonly IDeleteBusUseCase _deleteBusUseCase;
    private readonly IListBusesBasicUseCase _listBusesBasicUseCase;
    private readonly IGetBusDetailUseCase _getBusDetailUseCase;
    private readonly IGetBusAssignedToDriverUseCase _getBusAssignedToDriverUseCase;
    private readonly IAssignDriverToBusUseCase _assignDriverUseCase;
    private readonly IUnassignDriverFromBusUseCase _unassignDriverUseCase;
    private readonly SearchBusesService _searchBusesService;

    public BusController(
        ICreateBusUseCase createBusUseCase,
        IUpdateBusUseCase updateBusUseCase,
        IChangeBusStatusUseCase changeBusStatusUseCase,
        IDeleteBusUseCase deleteBusUseCase,
        IListBusesBasicUseCase listBusesBasicUseCase,
        IGetBusDetailUseCase getBusDetailUseCase,
        IGetBusAssignedToDriverUseCase getBusAssignedToDriverUseCase,
        IAssignDriverToBusUseCase assignDriverUseCase,
        IUnassignDriverFromBusUseCase unassignDriverUseCase,
        SearchBusesService searchBusesService)
    {
        _createBusUseCase = createBusUseCase;
        _updateBusUseCase = updateBusUseCase;
        _changeBusStatusUseCase = changeBusStatusUseCase;
        _deleteBusUseCase = deleteBusUseCase;
        _listBusesBasicUseCase = listBusesBasicUseCase;
        _getBusDetailUseCase = getBusDetailUseCase;
        _getBusAssignedToDriverUseCase = getBusAssignedToDriverUseCase;
        _assignDriverUseCase = assignDriverUseCase;
        _unassignDriverUseCase = unassignDriverUseCase;
        _searchBusesService = searchBusesService;
    }

    [EnableRateLimiting("writes")]
    [RequirePermission(FleetPermissions.BusesManage)]
    [HttpPost]
    public async Task<ActionResult<CreateBusResponse>> Create([FromBody] CreateBusRequest request, CancellationToken ct)
    {
        try
        {
            var response = await _createBusUseCase.ExecuteAsync(request, ct);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [EnableRateLimiting("writes")]
    [RequirePermission(FleetPermissions.BusesManage)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBusRequest request, CancellationToken ct)
    {
        try
        {
            await _updateBusUseCase.ExecuteAsync(id, request, ct);
        }
        catch (ArgumentException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status404NotFound);
        }
        return NoContent();
    }

    [EnableRateLimiting("writes")]
    [RequirePermission(FleetPermissions.BusesManage)]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusRequest request, CancellationToken ct)
    {
        try
        {
            await _changeBusStatusUseCase.ExecuteAsync(id, request, ct);
        }
        catch (ArgumentException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status404NotFound);
        }
        return NoContent();
    }

    [RequirePermission(FleetPermissions.BusesRead)]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BusListItemResponse>>> ListBasic(CancellationToken ct)
    {
        var buses = await _listBusesBasicUseCase.ExecuteAsync(ct);
        return Ok(buses);
    }

    [RequirePermission(FleetPermissions.BusesRead)]
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<BusListItemResponse>>> Search([FromQuery] string? search, CancellationToken ct)
    {
        var request = new SearchBusesRequest { SearchTerm = search };
        var buses = await _searchBusesService.SearchAsync(request, ct);
        return Ok(buses);
    }

    [RequirePermission(FleetPermissions.BusesRead)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BusDetailResponse>> GetDetail(Guid id, CancellationToken ct)
    {
        var bus = await _getBusDetailUseCase.ExecuteAsync(id, ct);
        return bus is null ? NotFound() : Ok(bus);
    }

    /// <summary>
    /// Bus asignado a un conductor: el propio conductor, o personal con buses.read
    /// (la regla de pertenencia vive en la capa de aplicacion).
    /// </summary>
    [HttpGet("assigned/{profileId:guid}")]
    public async Task<ActionResult<BusAssignedResponse>> GetAssignedToDriver(Guid profileId, CancellationToken ct)
    {
        try
        {
            var bus = await _getBusAssignedToDriverUseCase.ExecuteAsync(profileId, ct);
            return bus is null ? NotFound() : Ok(bus);
        }
        catch (BusAccessDeniedException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status403Forbidden);
        }
    }

    [EnableRateLimiting("writes")]
    [RequirePermission(FleetPermissions.BusesManage)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _deleteBusUseCase.ExecuteAsync(id, ct);
        return NoContent();
    }

    [EnableRateLimiting("writes")]
    [RequirePermission(FleetPermissions.DriversManage)]
    [HttpPut("{busId:guid}/driver")]
    public async Task<IActionResult> AssignDriver(Guid busId, [FromBody] AssignDriverRequest request, CancellationToken ct)
    {
        try
        {
            await _assignDriverUseCase.ExecuteAsync(busId, request, ct);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }

    [EnableRateLimiting("writes")]
    [RequirePermission(FleetPermissions.DriversManage)]
    [HttpDelete("{busId:guid}/driver")]
    public async Task<IActionResult> UnassignDriver(Guid busId, CancellationToken ct)
    {
        try
        {
            await _unassignDriverUseCase.ExecuteAsync(busId, ct);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }
}