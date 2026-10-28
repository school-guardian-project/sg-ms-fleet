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
        var useCase = new AssignDriverToBusService(busRepo, assignmentRepo, iamService, AccessPolicies.SuperAdmin());

        var bus = new Bus { Plate = "ABC123", Status = Status.Active };
        await busRepo.AddAsync(bus);

        await useCase.ExecuteAsync(bus.Id, new AssignDriverRequest { ProfileId = Guid.NewGuid() });

        var assignment = await assignmentRepo.GetActiveByBusIdAsync(bus.Id);
        Assert.That(assignment, Is.Not.Null);
    }

    [Test]
    public async Task AssignThenRead_UsesTheDriverProfile_AndClosingTheAssignmentRemovesTheBus()
    {
        var buses = new InMemoryBusRepository();
        var assignments = new InMemoryDriverAssignmentRepository();
        var bus = new Bus { Plate = "ABC123", Status = Status.Active };
        await buses.AddAsync(bus);
        var profileId = Guid.NewGuid();
        var otherProfileId = Guid.NewGuid();
        var assign = new AssignDriverToBusService(
            buses, assignments, new FakeIamService(), AccessPolicies.SuperAdmin());
        var lookup = new GetBusAssignedToDriverService(assignments, buses, new BusMapper());

        await assign.ExecuteAsync(bus.Id, new AssignDriverRequest { ProfileId = profileId });
        var saved = await lookup.ExecuteAsync(profileId);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Id, Is.EqualTo(bus.Id));
        Assert.That(saved.Plate, Is.EqualTo("ABC123"));
        Assert.That(await lookup.ExecuteAsync(otherProfileId), Is.Null);
        Assert.That((await assignments.GetActiveByBusIdAsync(bus.Id))!.ProfileId, Is.EqualTo(profileId));

        await assignments.CloseActiveAsync(bus.Id);
        Assert.That(await lookup.ExecuteAsync(profileId), Is.Null);
    }

    [Test]
    public async Task ExecuteAsync_WithInactiveBus_ShouldThrow()
    {
        var busRepo = new InMemoryBusRepository();
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var iamService = new FakeIamService();
        var useCase = new AssignDriverToBusService(busRepo, assignmentRepo, iamService, AccessPolicies.SuperAdmin());

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
        var useCase = new AssignDriverToBusService(busRepo, assignmentRepo, iamService, AccessPolicies.SuperAdmin());

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
        var useCase = new AssignDriverToBusService(busRepo, assignmentRepo, iamService, AccessPolicies.SuperAdmin());

        var bus = new Bus { Plate = "ABC123", Status = Status.Active };
        await busRepo.AddAsync(bus);

        await useCase.ExecuteAsync(bus.Id, new AssignDriverRequest { ProfileId = Guid.NewGuid() });

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecuteAsync(bus.Id, new AssignDriverRequest { ProfileId = Guid.NewGuid() }));
    }
}
