using Xunit;
using Xunit;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Tests.Fakes;

namespace ms_fleet.Api.Tests.Application;

public class AssignDriverToBusServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldAssign()
    {
        var busRepo = new InMemoryBusRepository();
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var iamService = new FakeIamService();
        var useCase = new AssignDriverToBusService(busRepo, assignmentRepo, iamService);

        var bus = new Bus { Plate = "ABC123", Status = Status.Active };
        await busRepo.AddAsync(bus);

        await useCase.ExecuteAsync(bus.Id, Guid.NewGuid());

        var assignment = await assignmentRepo.GetActiveByBusIdAsync(bus.Id);
        Assert.NotNull(assignment);
    }

    [Fact]
    public async Task ExecuteAsync_WithInactiveBus_ShouldThrow()
    {
        var busRepo = new InMemoryBusRepository();
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var iamService = new FakeIamService();
        var useCase = new AssignDriverToBusService(busRepo, assignmentRepo, iamService);

        var bus = new Bus { Plate = "ABC123", Status = Status.Inactive };
        await busRepo.AddAsync(bus);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecuteAsync(bus.Id, Guid.NewGuid()));
    }

    [Fact]
    public async Task ExecuteAsync_WithDriverAlreadyAssigned_ShouldThrow()
    {
        var busRepo = new InMemoryBusRepository();
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var iamService = new FakeIamService();
        var useCase = new AssignDriverToBusService(busRepo, assignmentRepo, iamService);

        var bus1 = new Bus { Plate = "ABC123", Status = Status.Active };
        var bus2 = new Bus { Plate = "DEF456", Status = Status.Active };
        await busRepo.AddAsync(bus1);
        await busRepo.AddAsync(bus2);

        var profileId = Guid.NewGuid();
        await useCase.ExecuteAsync(bus1.Id, profileId);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecuteAsync(bus2.Id, profileId));
    }

    [Fact]
    public async Task ExecuteAsync_WhenBusAlreadyHasDriver_ShouldThrow()
    {
        var busRepo = new InMemoryBusRepository();
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var iamService = new FakeIamService();
        var useCase = new AssignDriverToBusService(busRepo, assignmentRepo, iamService);

        var bus = new Bus { Plate = "ABC123", Status = Status.Active };
        await busRepo.AddAsync(bus);

        await useCase.ExecuteAsync(bus.Id, Guid.NewGuid());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecuteAsync(bus.Id, Guid.NewGuid()));
    }
}
