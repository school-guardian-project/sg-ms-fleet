using NUnit.Framework;
using ms_fleet.Api.Application.Dto.Request;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Tests.Fakes;

namespace ms_fleet.Api.Tests.Application;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class AssignDriverToBusServiceTests
{
    [Test]
    public async Task ExecuteAsync_WithValidData_ShouldAssign()
    {
        var busRepo = new InMemoryBusRepository();
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var iamService = new FakeIamService();
        var useCase = new AssignDriverToBusService(busRepo, assignmentRepo, iamService);

        var bus = new Bus { Plate = "ABC123", Status = Status.Active };
        await busRepo.AddAsync(bus);

        await useCase.ExecuteAsync(bus.Id, new AssignDriverRequest { ProfileId = Guid.NewGuid() });

        var assignment = await assignmentRepo.GetActiveByBusIdAsync(bus.Id);
        Assert.That(assignment, Is.Not.Null);
    }

    [Test]
    public async Task ExecuteAsync_WithInactiveBus_ShouldThrow()
    {
        var busRepo = new InMemoryBusRepository();
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var iamService = new FakeIamService();
        var useCase = new AssignDriverToBusService(busRepo, assignmentRepo, iamService);

        var bus = new Bus { Plate = "ABC123", Status = Status.Inactive };
        await busRepo.AddAsync(bus);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecuteAsync(bus.Id, new AssignDriverRequest { ProfileId = Guid.NewGuid() }));
    }

    [Test]
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
        await useCase.ExecuteAsync(bus1.Id, new AssignDriverRequest { ProfileId = profileId });

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecuteAsync(bus2.Id, new AssignDriverRequest { ProfileId = profileId }));
    }

    [Test]
    public async Task ExecuteAsync_WhenBusAlreadyHasDriver_ShouldThrow()
    {
        var busRepo = new InMemoryBusRepository();
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var iamService = new FakeIamService();
        var useCase = new AssignDriverToBusService(busRepo, assignmentRepo, iamService);

        var bus = new Bus { Plate = "ABC123", Status = Status.Active };
        await busRepo.AddAsync(bus);

        await useCase.ExecuteAsync(bus.Id, new AssignDriverRequest { ProfileId = Guid.NewGuid() });

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecuteAsync(bus.Id, new AssignDriverRequest { ProfileId = Guid.NewGuid() }));
    }
}
