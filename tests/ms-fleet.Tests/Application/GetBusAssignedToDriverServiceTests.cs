using ms_fleet.Api.Application.Dto.Request;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Tests.Fakes;
using NUnit.Framework;

namespace ms_fleet.Api.Tests.Application;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class GetBusAssignedToDriverServiceTests
{
    [Test]
    public async Task ExecuteAsync_WithActiveAssignment_ShouldReturnBus()
    {
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var busRepo = new InMemoryBusRepository();
        var useCase = new GetBusAssignedToDriverService(assignmentRepo, busRepo, new BusMapper(), AccessPolicies.SuperAdmin());

        var profileId = Guid.NewGuid();
        var bus = new Bus { Plate = "ABC123", CampuseId = Guid.NewGuid(), Status = Status.Active };
        await busRepo.AddAsync(bus);
        await assignmentRepo.AddAsync(new DriverAssignment
        {
            ProfileId = profileId,
            BusId = bus.Id,
            AssignedFrom = DateTime.UtcNow
        });

        var result = await useCase.ExecuteAsync(profileId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(bus.Id));
        Assert.That(result.Plate, Is.EqualTo("ABC123"));
        Assert.That(result.CampuseId, Is.EqualTo(bus.CampuseId));
    }

    [Test]
    public async Task ExecuteAsync_WithoutAssignment_ShouldReturnNull()
    {
        var useCase = new GetBusAssignedToDriverService(
            new InMemoryDriverAssignmentRepository(),
            new InMemoryBusRepository(),
            new BusMapper(), AccessPolicies.SuperAdmin());

        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ExecuteAsync_WithInactiveBus_ShouldReturnNull()
    {
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var busRepo = new InMemoryBusRepository();
        var useCase = new GetBusAssignedToDriverService(assignmentRepo, busRepo, new BusMapper(), AccessPolicies.SuperAdmin());

        var profileId = Guid.NewGuid();
        var bus = new Bus { Plate = "XYZ789", Status = Status.Inactive };
        await busRepo.AddAsync(bus);
        await assignmentRepo.AddAsync(new DriverAssignment
        {
            ProfileId = profileId,
            BusId = bus.Id,
            AssignedFrom = DateTime.UtcNow
        });

        var result = await useCase.ExecuteAsync(profileId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ExecuteAsync_WithClosedAssignment_ShouldReturnNull()
    {
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var busRepo = new InMemoryBusRepository();
        var useCase = new GetBusAssignedToDriverService(assignmentRepo, busRepo, new BusMapper(), AccessPolicies.SuperAdmin());

        var profileId = Guid.NewGuid();
        var bus = new Bus { Plate = "DEF456", Status = Status.Active };
        await busRepo.AddAsync(bus);
        await assignmentRepo.AddAsync(new DriverAssignment
        {
            ProfileId = profileId,
            BusId = bus.Id,
            AssignedFrom = DateTime.UtcNow.AddDays(-10),
            AssignedTo = DateTime.UtcNow.AddDays(-1)
        });

        var result = await useCase.ExecuteAsync(profileId);

        Assert.That(result, Is.Null);
    }
}
