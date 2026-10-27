using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Application.Security;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Tests.Fakes;
using NUnit.Framework;

namespace ms_fleet.Api.Tests.Application;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class BusAccessPolicyTests
{
    private static readonly Guid SchoolA = Guid.NewGuid();
    private static readonly Guid SchoolB = Guid.NewGuid();
    private static readonly Guid CampusA = Guid.NewGuid();
    private static readonly Guid CampusB = Guid.NewGuid();
    private const int DriverRole = 3;

    private readonly InMemoryBusRepository _buses = new();
    private readonly InMemoryDriverAssignmentRepository _assignments = new();
    private readonly InMemoryCampusReferenceRepository _campuses = new();

    [SetUp]
    public void SetUp()
    {
        _campuses.CampusIdsBySchool[SchoolA] = [CampusA];
        _campuses.CampusIdsBySchool[SchoolB] = [CampusB];
    }

    [Test]
    public async Task Conductor_ConsultaSuPropioBus_LoRecibe()
    {
        var driverId = Guid.NewGuid();
        var bus = await AssignBusAsync(driverId, CampusA);
        var policy = Policy(driverId, DriverRole, campusId: CampusA, permissions: [FleetPermissions.BusesRead]);

        var result = await AssignedService(policy).ExecuteAsync(driverId);

        Assert.That(result?.Id, Is.EqualTo(bus.Id));
    }

    [Test]
    public async Task Conductor_ConsultaElBusDeOtroConductor_EsDenegado()
    {
        var otherDriver = Guid.NewGuid();
        await AssignBusAsync(otherDriver, CampusA);
        // Tiene buses.read y es de la misma sede, pero no es personal del colegio.
        var policy = Policy(Guid.NewGuid(), DriverRole, campusId: CampusA, permissions: [FleetPermissions.BusesRead]);

        Assert.ThrowsAsync<BusAccessDeniedException>(() => AssignedService(policy).ExecuteAsync(otherDriver));
    }

    [Test]
    public void AdminSinBusesRead_ConsultaBusDeConductor_EsDenegado()
    {
        var policy = Policy(Guid.NewGuid(), FleetRoles.Admin, schoolId: SchoolA);

        Assert.ThrowsAsync<BusAccessDeniedException>(() => AssignedService(policy).ExecuteAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task Admin_ConsultaConductorDeSuColegio_LoRecibe()
    {
        var driverId = Guid.NewGuid();
        var bus = await AssignBusAsync(driverId, CampusA);
        var policy = Policy(Guid.NewGuid(), FleetRoles.Admin, schoolId: SchoolA, permissions: [FleetPermissions.BusesRead]);

        var result = await AssignedService(policy).ExecuteAsync(driverId);

        Assert.That(result?.Id, Is.EqualTo(bus.Id));
    }

    [Test]
    public async Task Admin_ConsultaConductorDeOtroColegio_NoLoVe()
    {
        var driverId = Guid.NewGuid();
        await AssignBusAsync(driverId, CampusB);
        var policy = Policy(Guid.NewGuid(), FleetRoles.Admin, schoolId: SchoolA, permissions: [FleetPermissions.BusesRead]);

        var result = await AssignedService(policy).ExecuteAsync(driverId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task SuperAdmin_ConsultaCualquierConductor_LoRecibe()
    {
        var driverId = Guid.NewGuid();
        var bus = await AssignBusAsync(driverId, CampusB);
        var policy = Policy(Guid.NewGuid(), FleetRoles.SuperAdmin, permissions: [FleetPermissions.BusesRead]);

        var result = await AssignedService(policy).ExecuteAsync(driverId);

        Assert.That(result?.Id, Is.EqualTo(bus.Id));
    }

    [Test]
    public async Task DetalleDeBus_DeOtroColegio_NoSeRevela()
    {
        var bus = await AddBusAsync(CampusB);
        var policy = Policy(Guid.NewGuid(), FleetRoles.Admin, schoolId: SchoolA, permissions: [FleetPermissions.BusesRead]);

        var result = await DetailService(policy).ExecuteAsync(bus.Id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DetalleDeBus_DeSuColegio_SeDevuelve()
    {
        var bus = await AddBusAsync(CampusA);
        var policy = Policy(Guid.NewGuid(), FleetRoles.Admin, schoolId: SchoolA, permissions: [FleetPermissions.BusesRead]);

        var result = await DetailService(policy).ExecuteAsync(bus.Id);

        Assert.That(result, Is.Not.Null);
    }

    private BusAccessPolicy Policy(
        Guid profileId, int roleId, Guid? schoolId = null, Guid? campusId = null, string[]? permissions = null)
    {
        var tenant = new FakeTenantProvider
        {
            RoleId = roleId,
            SchoolId = schoolId,
            CampusId = campusId,
            ShouldFilter = roleId == FleetRoles.Admin ? schoolId is not null
                : roleId == FleetRoles.SuperAdmin ? false
                : campusId is not null,
        };
        return new BusAccessPolicy(new FakeCurrentUser { ProfileId = profileId, Permissions = permissions ?? [] }, tenant, _campuses);
    }

    private GetBusAssignedToDriverService AssignedService(BusAccessPolicy policy) =>
        new(_assignments, _buses, new BusMapper(), policy);

    private GetBusDetailService DetailService(BusAccessPolicy policy) =>
        new(_buses, _assignments, new FakeIamService(), new FakeGpsDeviceService(), new BusMapper(), policy);

    private async Task<Bus> AddBusAsync(Guid campusId)
    {
        var bus = new Bus { Plate = $"P{Random.Shared.Next(1000, 9999)}", CampuseId = campusId, Status = Status.Active, ModelId = 1 };
        await _buses.AddAsync(bus);
        return bus;
    }

    private async Task<Bus> AssignBusAsync(Guid driverId, Guid campusId)
    {
        var bus = await AddBusAsync(campusId);
        await _assignments.AddAsync(new DriverAssignment { ProfileId = driverId, BusId = bus.Id, AssignedFrom = DateTime.UtcNow });
        return bus;
    }
}
