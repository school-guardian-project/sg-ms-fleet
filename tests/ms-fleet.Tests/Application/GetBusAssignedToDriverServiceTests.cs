using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Tests.Fakes;
using Xunit;

namespace ms_fleet.Api.Tests.Application;

public class GetBusAssignedToDriverServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithActiveAssignment_ShouldReturnBus()
    {
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var busRepo = new InMemoryBusRepository();
        var useCase = new GetBusAssignedToDriverService(assignmentRepo, busRepo);

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

        Assert.NotNull(result);
        Assert.Equal(bus.Id, result.Id);
        Assert.Equal("ABC123", result.Plate);
        Assert.Equal(bus.CampuseId, result.CampuseId);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutAssignment_ShouldReturnNull()
    {
        var useCase = new GetBusAssignedToDriverService(
            new InMemoryDriverAssignmentRepository(),
            new InMemoryBusRepository());

        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAsync_WithInactiveBus_ShouldReturnNull()
    {
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var busRepo = new InMemoryBusRepository();
        var useCase = new GetBusAssignedToDriverService(assignmentRepo, busRepo);

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

        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAsync_WithClosedAssignment_ShouldReturnNull()
    {
        var assignmentRepo = new InMemoryDriverAssignmentRepository();
        var busRepo = new InMemoryBusRepository();
        var useCase = new GetBusAssignedToDriverService(assignmentRepo, busRepo);

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

        Assert.Null(result);
    }
}
