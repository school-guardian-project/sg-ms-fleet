using NUnit.Framework;
using ms_fleet.Api.Application.Dto.Request;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Tests.Fakes;

namespace ms_fleet.Api.Tests.Application;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class CreateBusServiceTests
{
    [Test]
    public async Task ExecuteAsync_WithValidData_ShouldReturnBusId()
    {
        var repo = new InMemoryBusRepository();
        var gpsService = new FakeGpsDeviceService();
        var useCase = new CreateBusService(repo, gpsService, new BusRequestMapper(), AccessPolicies.SuperAdmin());

        var response = await useCase.ExecuteAsync(NewRequest(capacity: 40, plate: "ABC123", modelId: 1));

        Assert.That(response.BusId, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task ExecuteAsync_WithDuplicatePlate_ShouldThrow()
    {
        var repo = new InMemoryBusRepository();
        var gpsService = new FakeGpsDeviceService();
        var useCase = new CreateBusService(repo, gpsService, new BusRequestMapper(), AccessPolicies.SuperAdmin());

        await useCase.ExecuteAsync(NewRequest(capacity: 40, plate: "ABC123", modelId: 1));

        Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(NewRequest(capacity: 40, plate: "ABC123", modelId: 1)));
    }

    [Test]
    public async Task ExecuteAsync_WithZeroCapacity_ShouldThrow()
    {
        var repo = new InMemoryBusRepository();
        var gpsService = new FakeGpsDeviceService();
        var useCase = new CreateBusService(repo, gpsService, new BusRequestMapper(), AccessPolicies.SuperAdmin());

        Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(NewRequest(capacity: 0, plate: "ABC123", modelId: 1)));
    }

    private static CreateBusRequest NewRequest(byte capacity, string plate, int modelId) => new()
    {
        CampuseId = Guid.NewGuid(),
        SoatValidity = DateTime.UtcNow.AddYears(1),
        GpsDeviceId = Guid.NewGuid(),
        Capacity = capacity,
        Plate = plate,
        ModelId = modelId,
    };
}
