using Microsoft.AspNetCore.Mvc;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Domain.Ports.Out;
using ms_fleet.Api.Infrastructure.Security;

namespace ms_fleet.Api.Infrastructure.Controller;

[ApiController]
[Route("api/gps-devices")]
[RequireProfile]
public class GpsDeviceController : ControllerBase
{
    private readonly IGpsDeviceService _gpsDeviceService;

    public GpsDeviceController(IGpsDeviceService gpsDeviceService)
    {
        _gpsDeviceService = gpsDeviceService;
    }

    /// <summary>Dispositivos GPS con el bus activo que los lleva (si lo hay).</summary>
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var devices = await _gpsDeviceService.ListAsync(ct);
        return Ok(GpsDeviceMapper.ToDtos(devices));
    }
}
