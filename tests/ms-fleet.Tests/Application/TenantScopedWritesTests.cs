using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ms_fleet.Api.Application.Dto.Request;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Application.Security;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Infrastructure.Auth;
using ms_fleet.Api.Tests.Fakes;
using NUnit.Framework;

namespace ms_fleet.Api.Tests.Application;

/// <summary>
/// Escrituras acotadas al colegio del admin, filtro de tenant que falla cerrado y
/// la consulta interna (gRPC) que no depende del usuario.
/// </summary>
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class TenantScopedWritesTests
{
    private static readonly Guid SchoolA = Guid.NewGuid();
    private static readonly Guid SchoolB = Guid.NewGuid();
    private static readonly Guid CampusA = Guid.NewGuid();
    private static readonly Guid CampusB = Guid.NewGuid();

    private readonly InMemoryBusRepository _buses = new();
    private readonly InMemoryDriverAssignmentRepository _assignments = new();
    private readonly InMemoryCampusReferenceRepository _campuses = new();

    [SetUp]
    public void SetUp()
    {
        _campuses.CampusIdsBySchool[SchoolA] = [CampusA];
        _campuses.CampusIdsBySchool[SchoolB] = [CampusB];
    }

    // --- Regresion: el gRPC interno (ms-route) no trae usuario ---

    [Test]
    public async Task ConsultaInterna_SinUsuario_DevuelveElBusDelConductor()
    {
        var driverId = Guid.NewGuid();
        var bus = await AddBusAsync(CampusA);
        await _assignments.AddAsync(new DriverAssignment { ProfileId = driverId, BusId = bus.Id, AssignedFrom = DateTime.UtcNow });

        var result = await new GetBusAssignedToDriverService(_assignments, _buses, new BusMapper()).ExecuteAsync(driverId);

        Assert.That(result?.Id, Is.EqualTo(bus.Id));
    }

    [Test]
    public void ConsultaRest_SinUsuario_SeRechaza()
    {
        var anonymous = new BusAccessPolicy(new FakeCurrentUser(), new FakeTenantProvider(), _campuses);
        var rest = new AuthorizedGetBusAssignedToDriverService(
            new GetBusAssignedToDriverService(_assignments, _buses, new BusMapper()), anonymous);

        Assert.ThrowsAsync<BusAccessDeniedException>(() => rest.ExecuteAsync(Guid.NewGuid()));
    }

    // --- Escrituras sobre buses de otro colegio ---

    [Test]
    public async Task Editar_BusDeOtroColegio_Devuelve403()
    {
        var bus = await AddBusAsync(CampusB);

        Assert.ThrowsAsync<BusAccessDeniedException>(() =>
            Update(AdminOf(SchoolA)).ExecuteAsync(bus.Id, new UpdateBusRequest { Plate = "NEW123" }));
    }

    [Test]
    public async Task Editar_BusDeSuColegio_SeGuarda()
    {
        var bus = await AddBusAsync(CampusA);

        await Update(AdminOf(SchoolA)).ExecuteAsync(bus.Id, new UpdateBusRequest { Plate = "NEW123" });

        Assert.That((await _buses.GetByIdAsync(bus.Id))!.Plate, Is.EqualTo("NEW123"));
    }

    [Test]
    public async Task Editar_MoverBusAUnaSedeDeOtroColegio_Devuelve403()
    {
        var bus = await AddBusAsync(CampusA);

        Assert.ThrowsAsync<BusAccessDeniedException>(() =>
            Update(AdminOf(SchoolA)).ExecuteAsync(bus.Id, new UpdateBusRequest { CampuseId = CampusB }));
        Assert.That((await _buses.GetByIdAsync(bus.Id))!.CampuseId, Is.EqualTo(CampusA));
    }

    [Test]
    public async Task CambiarEstado_BusDeOtroColegio_Devuelve403()
    {
        var bus = await AddBusAsync(CampusB);

        Assert.ThrowsAsync<BusAccessDeniedException>(() =>
            new ChangeBusStatusService(_buses, AdminOf(SchoolA)).ExecuteAsync(bus.Id, new ChangeStatusRequest { Status = "Inactive" }));
        Assert.That((await _buses.GetByIdAsync(bus.Id))!.Status, Is.EqualTo(Status.Active));
    }

    [Test]
    public async Task Borrar_BusDeOtroColegio_Devuelve403()
    {
        var bus = await AddBusAsync(CampusB);

        Assert.ThrowsAsync<BusAccessDeniedException>(() =>
            new DeleteBusService(_buses, AdminOf(SchoolA)).ExecuteAsync(bus.Id));
        Assert.That((await _buses.GetByIdAsync(bus.Id))!.Status, Is.EqualTo(Status.Active));
    }

    [Test]
    public async Task AsignarConductor_BusDeOtroColegio_Devuelve403()
    {
        var bus = await AddBusAsync(CampusB);

        Assert.ThrowsAsync<BusAccessDeniedException>(() =>
            new AssignDriverToBusService(_buses, _assignments, new FakeIamService(), AdminOf(SchoolA))
                .ExecuteAsync(bus.Id, new AssignDriverRequest { ProfileId = Guid.NewGuid() }));
        Assert.That(await _assignments.GetActiveByBusIdAsync(bus.Id), Is.Null);
    }

    [Test]
    public async Task QuitarConductor_BusDeOtroColegio_Devuelve403()
    {
        var bus = await AddBusAsync(CampusB);
        await _assignments.AddAsync(new DriverAssignment { ProfileId = Guid.NewGuid(), BusId = bus.Id, AssignedFrom = DateTime.UtcNow });

        Assert.ThrowsAsync<BusAccessDeniedException>(() =>
            new UnassignDriverFromBusService(_assignments, _buses, AdminOf(SchoolA)).ExecuteAsync(bus.Id));
        Assert.That(await _assignments.GetActiveByBusIdAsync(bus.Id), Is.Not.Null);
    }

    [Test]
    public async Task QuitarConductor_BusDeSuColegio_LoQuita()
    {
        var bus = await AddBusAsync(CampusA);
        await _assignments.AddAsync(new DriverAssignment { ProfileId = Guid.NewGuid(), BusId = bus.Id, AssignedFrom = DateTime.UtcNow });

        await new UnassignDriverFromBusService(_assignments, _buses, AdminOf(SchoolA)).ExecuteAsync(bus.Id);

        Assert.That(await _assignments.GetActiveByBusIdAsync(bus.Id), Is.Null);
    }

    [Test]
    public void Crear_EnSedeDeOtroColegio_Devuelve403()
    {
        Assert.ThrowsAsync<BusAccessDeniedException>(() =>
            Create(AdminOf(SchoolA)).ExecuteAsync(NewBus(CampusB)));
    }

    [Test]
    public async Task Crear_EnSedeDeSuColegio_LoCrea()
    {
        var response = await Create(AdminOf(SchoolA)).ExecuteAsync(NewBus(CampusA));

        Assert.That(await _buses.GetByIdAsync(response.BusId), Is.Not.Null);
    }

    [Test]
    public async Task SuperAdmin_EditaBusesDeCualquierColegio()
    {
        var bus = await AddBusAsync(CampusB);

        await Update(AccessPolicies.SuperAdmin()).ExecuteAsync(bus.Id, new UpdateBusRequest { Plate = "SUP123" });

        Assert.That((await _buses.GetByIdAsync(bus.Id))!.Plate, Is.EqualTo("SUP123"));
    }

    [Test]
    public async Task AdminSinColegio_NoGestionaNingunBus()
    {
        var bus = await AddBusAsync(CampusA);
        var adminWithoutSchool = Policy(FleetRoles.Admin, schoolId: null);

        Assert.ThrowsAsync<BusAccessDeniedException>(() =>
            new DeleteBusService(_buses, adminWithoutSchool).ExecuteAsync(bus.Id));
    }

    // --- HttpContextTenantProvider: falla cerrado ---

    [Test]
    public void TokenSinRoleId_SeFiltra()
    {
        var tenant = TenantFor(new Claim(SgClaimTypes.Subject, Guid.NewGuid().ToString()));

        Assert.That(tenant.ShouldFilter, Is.True);
    }

    [Test]
    public void RolDesconocido_SeFiltra()
    {
        var tenant = TenantFor(new Claim(SgClaimTypes.RoleId, "9"));

        Assert.That(tenant.ShouldFilter, Is.True);
    }

    [Test]
    public void AdminSinSchoolId_SeFiltra()
    {
        var tenant = TenantFor(new Claim(SgClaimTypes.RoleId, "1"));

        Assert.That(tenant.ShouldFilter, Is.True);
    }

    [Test]
    public void SuperAdmin_NoSeFiltra()
    {
        var tenant = TenantFor(new Claim(SgClaimTypes.RoleId, "5"));

        Assert.That(tenant.ShouldFilter, Is.False);
    }

    [Test]
    public void SinUsuario_LlamadaInterna_NoSeFiltra()
    {
        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };

        Assert.That(new HttpContextTenantProvider(accessor).ShouldFilter, Is.False);
    }

    private static HttpContextTenantProvider TenantFor(params Claim[] claims)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = user } };
        return new HttpContextTenantProvider(accessor);
    }

    private BusAccessPolicy AdminOf(Guid schoolId) => Policy(FleetRoles.Admin, schoolId);

    private BusAccessPolicy Policy(int roleId, Guid? schoolId) => new(
        new FakeCurrentUser
        {
            ProfileId = Guid.NewGuid(),
            Permissions = [FleetPermissions.BusesRead, FleetPermissions.BusesManage, FleetPermissions.DriversManage],
        },
        new FakeTenantProvider { RoleId = roleId, SchoolId = schoolId, ShouldFilter = roleId != FleetRoles.SuperAdmin },
        _campuses);

    private UpdateBusService Update(BusAccessPolicy policy) =>
        new(_buses, new FakeGpsDeviceService(), new BusRequestMapper(), policy);

    private CreateBusService Create(BusAccessPolicy policy) =>
        new(_buses, new FakeGpsDeviceService(), new BusRequestMapper(), policy);

    private static CreateBusRequest NewBus(Guid campusId) => new()
    {
        CampuseId = campusId,
        SoatValidity = DateTime.UtcNow.AddYears(1),
        GpsDeviceId = Guid.NewGuid(),
        Capacity = 40,
        Plate = $"T{Random.Shared.Next(10000, 99999)}",
        ModelId = 1,
    };

    private async Task<Bus> AddBusAsync(Guid campusId)
    {
        var bus = new Bus { Plate = $"B{Random.Shared.Next(10000, 99999)}", CampuseId = campusId, Status = Status.Active };
        await _buses.AddAsync(bus);
        return bus;
    }
}
