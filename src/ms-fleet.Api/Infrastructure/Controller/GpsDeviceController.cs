using ms_fleet.Api.Infrastructure.Auth;
using ms_fleet.Api.Domain.Model;
﻿using Microsoft.AspNetCore.Mvc;
using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Infrastructure.Controller;

[ApiController]
[Route("api/gps-devices")]
public class GpsDeviceController : ControllerBase
{
    private readonly IGpsDeviceService _gpsDeviceService;
    private readonly IGpsDeviceMapper _gpsDeviceMapper;

    public GpsDeviceController(IGpsDeviceService gpsDeviceService, IGpsDeviceMapper gpsDeviceMapper)
    {
        _gpsDeviceService = gpsDeviceService;
        _gpsDeviceMapper = gpsDeviceMapper;
    }

    /// <summary>Dispositivos GPS con el bus activo que los lleva (si lo hay).</summary>
    [RequirePermission(FleetPermissions.BusesManage)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GpsDeviceResponse>>> List(CancellationToken ct)
    {
        var devices = await _gpsDeviceService.ListAsync(ct);
        return Ok(_gpsDeviceMapper.ToResponses(devices));
    }
}