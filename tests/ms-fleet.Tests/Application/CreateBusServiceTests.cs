using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Tests.Fakes;

namespace ms_fleet.Api.Tests.Application;

public class CreateBusServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldReturnBusId()
    {
        var repo = new InMemoryBusRepository();
        var gpsService = new FakeGpsDeviceService();
        var useCase = new CreateBusService(repo, gpsService);

        var busId = await useCase.ExecuteAsync(
            Guid.NewGuid(), DateTime.UtcNow.AddYears(1), Guid.NewGuid(),
            40, "ABC123", 1);

        Assert.NotEqual(Guid.Empty, busId);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicatePlate_ShouldThrow()
    {
        var repo = new InMemoryBusRepository();
        var gpsService = new FakeGpsDeviceService();
        var useCase = new CreateBusService(repo, gpsService);

        await useCase.ExecuteAsync(
            Guid.NewGuid(), DateTime.UtcNow.AddYears(1), Guid.NewGuid(),
            40, "ABC123", 1);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(
                Guid.NewGuid(), DateTime.UtcNow.AddYears(1), Guid.NewGuid(),
                40, "ABC123", 1));
    }

    [Fact]
    public async Task ExecuteAsync_WithZeroCapacity_ShouldThrow()
    {
        var repo = new InMemoryBusRepository();
        var gpsService = new FakeGpsDeviceService();
        var useCase = new CreateBusService(repo, gpsService);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(
                Guid.NewGuid(), DateTime.UtcNow.AddYears(1), Guid.NewGuid(),
                0, "ABC123", 1));
    }
}
