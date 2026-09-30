using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.In;

namespace ms_fleet.Api.Infrastructure.Controller;

[ApiController]
[Route("api/buses")]
public class BusController : ControllerBase
{
    private readonly ICreateBusUseCase _createBusUseCase;
    private readonly IUpdateBusUseCase _updateBusUseCase;
    private readonly IChangeBusStatusUseCase _changeBusStatusUseCase;
    private readonly IGetBusUseCase _getBusUseCase;
    private readonly IListBusesUseCase _listBusesUseCase;
    private readonly IAssignDriverToBusUseCase _assignDriverUseCase;
    private readonly IUnassignDriverFromBusUseCase _unassignDriverUseCase;

    public BusController(
        ICreateBusUseCase createBusUseCase,
        IUpdateBusUseCase updateBusUseCase,
        IChangeBusStatusUseCase changeBusStatusUseCase,
        IGetBusUseCase getBusUseCase,
        IListBusesUseCase listBusesUseCase,
        IAssignDriverToBusUseCase assignDriverUseCase,
        IUnassignDriverFromBusUseCase unassignDriverUseCase)
    {
        _createBusUseCase = createBusUseCase;
        _updateBusUseCase = updateBusUseCase;
        _changeBusStatusUseCase = changeBusStatusUseCase;
        _getBusUseCase = getBusUseCase;
        _listBusesUseCase = listBusesUseCase;
        _assignDriverUseCase = assignDriverUseCase;
        _unassignDriverUseCase = unassignDriverUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBusRequest request, CancellationToken ct)
    {
        var busId = await _createBusUseCase.ExecuteAsync(
            request.CampuseId, request.SoatValidity, request.GpsDeviceId,
            request.Capacity, request.Plate, request.ModelId, ct);
        return Ok(busId);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBusRequest request, CancellationToken ct)
    {
        await _updateBusUseCase.ExecuteAsync(id, request.CampuseId, request.SoatValidity, request.Capacity, request.ModelId, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<Status>(request.Status, out var status))
            return BadRequest("Invalid status");

        await _changeBusStatusUseCase.ExecuteAsync(id, status, ct);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var buses = await _listBusesUseCase.ExecuteAsync(ct);
        return Ok(buses);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var bus = await _getBusUseCase.ExecuteAsync(id, ct);
        return bus is null ? NotFound() : Ok(bus);
    }

    [HttpPut("{busId:guid}/driver")]
    public async Task<IActionResult> AssignDriver(Guid busId, [FromBody] AssignDriverRequest request, CancellationToken ct)
    {
        await _assignDriverUseCase.ExecuteAsync(busId, request.ProfileId, ct);
        return NoContent();
    }

    [HttpDelete("{busId:guid}/driver")]
    public async Task<IActionResult> UnassignDriver(Guid busId, CancellationToken ct)
    {
        await _unassignDriverUseCase.ExecuteAsync(busId, ct);
        return NoContent();
    }

    public class CreateBusRequest
    {
        public Guid CampuseId { get; set; }
        public DateTime SoatValidity { get; set; }
        public Guid GpsDeviceId { get; set; }
        public byte Capacity { get; set; }
        public string Plate { get; set; } = string.Empty;
        public int ModelId { get; set; }
    }

    public class UpdateBusRequest
    {
        public Guid CampuseId { get; set; }
        public DateTime SoatValidity { get; set; }
        public byte Capacity { get; set; }
        public int ModelId { get; set; }
    }

    public class ChangeStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class AssignDriverRequest
    {
        public Guid ProfileId { get; set; }
    }
}
