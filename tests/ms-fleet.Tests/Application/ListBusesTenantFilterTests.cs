using Xunit;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;
using ms_fleet.Api.Tests.Fakes;

namespace ms_fleet.Api.Tests.Application;

public class ListBusesTenantFilterTests
{
    private static readonly Guid CampusA = Guid.Parse("a3000001-0000-4000-8000-000000000001");
    private static readonly Guid CampusB = Guid.Parse("a3000006-0000-4000-8000-000000000006");
    private static readonly Guid SchoolX = Guid.Parse("a2000001-0000-4000-8000-000000000001");

    private static Bus NewBus(Guid campusId) => new()
    {
        CampuseId = campusId,
        Plate = $"PLQ-{campusId.ToString()[^4..]}",
        ModelId = 7,
        Capacity = 40,
        SoatValidity = new DateTime(2030, 1, 1),
        GpsDeviceId = Guid.NewGuid(),
        Status = Status.Active
    };

    private static async Task<(ListBusesBasicService Service, InMemoryBusRepository BusRepo, InMemoryCampusReferenceRepository CampusRepo)>
        CreateServiceAsync(ITenantProvider tenant)
    {
        var busRepo = new InMemoryBusRepository();
        await busRepo.AddAsync(NewBus(CampusA));
        await busRepo.AddAsync(NewBus(CampusB));
        busRepo.ModelNames[7] = new ModelNames("Mercedes", "O500");

        var campusRepo = new InMemoryCampusReferenceRepository();
        campusRepo.CampusIdsBySchool[SchoolX] = [CampusA];

        var service = new ListBusesBasicService(
            busRepo, new InMemoryDriverAssignmentRepository(), new FakeIamService(), tenant, campusRepo);

        return (service, busRepo, campusRepo);
    }

    [Fact]
    public async Task NoTenant_DoesNotFilter()
    {
        var (service, _, _) = await CreateServiceAsync(new FakeTenantProvider());

        var result = await service.ExecuteAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task DriverWithCampus_SeesOnlyItsCampus()
    {
        var (service, _, _) = await CreateServiceAsync(new FakeTenantProvider
        {
            RoleId = 3,
            CampusId = CampusA,
            ShouldFilter = true
        });

        var result = await service.ExecuteAsync();

        Assert.All(result, bus => Assert.Equal(CampusA, bus.CampuseId));
        Assert.Single(result);
    }

    [Fact]
    public async Task AdminWithSchool_SeesOnlySchoolCampuses()
    {
        var (service, _, _) = await CreateServiceAsync(new FakeTenantProvider
        {
            RoleId = 1,
            SchoolId = SchoolX,
            ShouldFilter = true
        });

        var result = await service.ExecuteAsync();

        Assert.All(result, bus => Assert.Equal(CampusA, bus.CampuseId));
        Assert.Single(result);
    }

    [Fact]
    public async Task SuperAdmin_DoesNotFilter()
    {
        var (service, _, _) = await CreateServiceAsync(new FakeTenantProvider
        {
            RoleId = 5,
            ShouldFilter = false
        });

        var result = await service.ExecuteAsync();

        Assert.Equal(2, result.Count);
    }
}
