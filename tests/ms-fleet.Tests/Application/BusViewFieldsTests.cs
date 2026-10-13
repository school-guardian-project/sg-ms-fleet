using NUnit.Framework;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Tests.Fakes;

namespace ms_fleet.Api.Tests.Application;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class BusViewFieldsTests
{
    private static Bus NewBus(byte modelId) => new()
    {
        CampuseId = Guid.NewGuid(),
        Plate = "ABC123",
        ModelId = modelId,
        Capacity = 40,
        SoatValidity = new DateTime(2030, 1, 1),
        GpsDeviceId = Guid.NewGuid(),
        Status = Status.Active
    };

    [Test]
    public async Task List_FillsBrandModelAndCampuseId()
    {
        var busRepo = new InMemoryBusRepository();
        var bus = NewBus(7);
        await busRepo.AddAsync(bus);
        busRepo.ModelNames[7] = new ModelNames("Mercedes", "O500");

        var service = new ListBusesBasicService(
            busRepo, new InMemoryDriverAssignmentRepository(), new FakeIamService(), new FakeTenantProvider(), new InMemoryCampusReferenceRepository());

        var resultItems = await service.ExecuteAsync();
        Assert.That(resultItems, Has.Exactly(1).Items);
        var result = resultItems.Single();

        Assert.That(result.CampuseId, Is.EqualTo(bus.CampuseId));
        Assert.That(result.Brand, Is.EqualTo("Mercedes"));
        Assert.That(result.Model, Is.EqualTo("O500"));
        Assert.That(result.DriverName, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task List_WithAssignedDriver_FillsDriverName()
    {
        var busRepo = new InMemoryBusRepository();
        var bus = NewBus(1);
        await busRepo.AddAsync(bus);

        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        await assignmentRepo.AddAsync(new DriverAssignment { BusId = bus.Id, ProfileId = Guid.NewGuid() });

        var service = new ListBusesBasicService(busRepo, assignmentRepo, new FakeIamService(), new FakeTenantProvider(), new InMemoryCampusReferenceRepository());

        var resultItems = await service.ExecuteAsync();
        Assert.That(resultItems, Has.Exactly(1).Items);
        var result = resultItems.Single();

        Assert.That(result.DriverName, Is.EqualTo("Test Driver"));
    }

    [Test]
    public async Task Detail_FillsBrandAndModel()
    {
        var busRepo = new InMemoryBusRepository();
        var bus = NewBus(3);
        await busRepo.AddAsync(bus);
        busRepo.ModelNames[3] = new ModelNames("Hino", "300");

        var service = new GetBusDetailService(
            busRepo, new InMemoryDriverAssignmentRepository(), new FakeIamService(), new FakeGpsDeviceService());

        var result = await service.ExecuteAsync(bus.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Brand, Is.EqualTo("Hino"));
        Assert.That(result.Model, Is.EqualTo("300"));
        Assert.That(result.CampuseId, Is.EqualTo(bus.CampuseId));
    }

    [Test]
    public async Task Detail_UnknownModel_LeavesBrandAndModelEmpty()
    {
        var busRepo = new InMemoryBusRepository();
        var bus = NewBus(9);
        await busRepo.AddAsync(bus);

        var service = new GetBusDetailService(
            busRepo, new InMemoryDriverAssignmentRepository(), new FakeIamService(), new FakeGpsDeviceService());

        var result = await service.ExecuteAsync(bus.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Brand, Is.EqualTo(string.Empty));
        Assert.That(result.Model, Is.EqualTo(string.Empty));
    }
}
