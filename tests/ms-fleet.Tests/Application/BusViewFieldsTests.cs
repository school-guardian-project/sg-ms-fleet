using Xunit;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Tests.Fakes;

namespace ms_fleet.Api.Tests.Application;

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

    [Fact]
    public async Task List_FillsBrandModelAndCampuseId()
    {
        var busRepo = new InMemoryBusRepository();
        var bus = NewBus(7);
        await busRepo.AddAsync(bus);
        busRepo.ModelNames[7] = new ModelNames("Mercedes", "O500");

        var service = new ListBusesBasicService(
            busRepo, new InMemoryDriverAssignmentRepository(), new FakeIamService(), new FakeTenantProvider(), new InMemoryCampusReferenceRepository());

        var result = Assert.Single(await service.ExecuteAsync());

        Assert.Equal(bus.CampuseId, result.CampuseId);
        Assert.Equal("Mercedes", result.Brand);
        Assert.Equal("O500", result.Model);
        Assert.Equal(string.Empty, result.DriverName);
    }

    [Fact]
    public async Task List_WithAssignedDriver_FillsDriverName()
    {
        var busRepo = new InMemoryBusRepository();
        var bus = NewBus(1);
        await busRepo.AddAsync(bus);

        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        await assignmentRepo.AddAsync(new DriverAssignment { BusId = bus.Id, ProfileId = Guid.NewGuid() });

        var service = new ListBusesBasicService(busRepo, assignmentRepo, new FakeIamService(), new FakeTenantProvider(), new InMemoryCampusReferenceRepository());

        var result = Assert.Single(await service.ExecuteAsync());

        Assert.Equal("Test Driver", result.DriverName);
    }

    [Fact]
    public async Task Detail_FillsBrandAndModel()
    {
        var busRepo = new InMemoryBusRepository();
        var bus = NewBus(3);
        await busRepo.AddAsync(bus);
        busRepo.ModelNames[3] = new ModelNames("Hino", "300");

        var service = new GetBusDetailService(
            busRepo, new InMemoryDriverAssignmentRepository(), new FakeIamService(), new FakeGpsDeviceService());

        var result = await service.ExecuteAsync(bus.Id);

        Assert.NotNull(result);
        Assert.Equal("Hino", result.Brand);
        Assert.Equal("300", result.Model);
        Assert.Equal(bus.CampuseId, result.CampuseId);
    }

    [Fact]
    public async Task Detail_UnknownModel_LeavesBrandAndModelEmpty()
    {
        var busRepo = new InMemoryBusRepository();
        var bus = NewBus(9);
        await busRepo.AddAsync(bus);

        var service = new GetBusDetailService(
            busRepo, new InMemoryDriverAssignmentRepository(), new FakeIamService(), new FakeGpsDeviceService());

        var result = await service.ExecuteAsync(bus.Id);

        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Brand);
        Assert.Equal(string.Empty, result.Model);
    }
}
