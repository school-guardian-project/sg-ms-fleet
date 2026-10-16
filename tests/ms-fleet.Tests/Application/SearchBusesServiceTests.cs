using NUnit.Framework;
using ms_fleet.Api.Application.UseCase;
using ms_fleet.Api.Application.UseCase.Strategy;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;
using ms_fleet.Api.Tests.Fakes;

namespace ms_fleet.Api.Tests.Application;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class SearchBusesServiceTests
{
    private static Bus NewBus(string plate, byte modelId) => new()
    {
        CampuseId = Guid.NewGuid(),
        Plate = plate,
        ModelId = modelId,
        Capacity = 40,
        SoatValidity = new DateTime(2030, 1, 1),
        GpsDeviceId = Guid.NewGuid(),
        Status = Status.Active
    };

    private static async Task<(SearchBusesService Service, Bus Mercedes, Bus Hino)> CreateServiceAsync()
    {
        var busRepo = new InMemoryBusRepository();
        var assignmentRepo = new InMemoryDriverAssignmentRepository();

        var mercedes = NewBus("ABC123", 7);
        await busRepo.AddAsync(mercedes);
        busRepo.ModelNames[7] = new ModelNames("Mercedes", "O500");
        await assignmentRepo.AddAsync(new DriverAssignment { BusId = mercedes.Id, ProfileId = Guid.NewGuid() });

        var hino = NewBus("DEF456", 9);
        await busRepo.AddAsync(hino);
        busRepo.ModelNames[9] = new ModelNames("Hino", "300");

        var listUseCase = new ListBusesBasicService(busRepo, assignmentRepo, new FakeIamService(), new FakeTenantProvider(), new InMemoryCampusReferenceRepository());
        var service = new SearchBusesService(listUseCase, new IBusSearchStrategy[]
        {
            new PlateSearchStrategy(),
            new NameSearchStrategy()
        });

        return (service, mercedes, hino);
    }

    [Test]
    public async Task Search_EmptyOrWhitespaceTerm_ReturnsEmpty()
    {
        var (service, _, _) = await CreateServiceAsync();

        Assert.That(await service.SearchAsync(string.Empty), Is.Empty);
        Assert.That(await service.SearchAsync("   "), Is.Empty);
    }

    [Test]
    public async Task Search_PlateShapedTerm_FiltersByPlate()
    {
        var (service, mercedes, hino) = await CreateServiceAsync();

        var result = await service.SearchAsync("DEF456");

        Assert.That(result.Single().Id, Is.EqualTo(hino.Id));

        var lowerCase = await service.SearchAsync("abc123");

        Assert.That(lowerCase.Single().Id, Is.EqualTo(mercedes.Id));
    }

    [Test]
    public async Task Search_GenericTerm_FiltersByBrandModelAndDriver()
    {
        var (service, mercedes, hino) = await CreateServiceAsync();

        var byBrand = await service.SearchAsync("mercedes");
        Assert.That(byBrand.Single().Id, Is.EqualTo(mercedes.Id));

        var byModel = await service.SearchAsync("300");
        Assert.That(byModel.Single().Id, Is.EqualTo(hino.Id));

        var byDriver = await service.SearchAsync("Test Driver");
        Assert.That(byDriver.Single().Id, Is.EqualTo(mercedes.Id));
    }

    [Test]
    public async Task Search_CatchAllTerm_IsCaseInsensitive()
    {
        var (service, _, hino) = await CreateServiceAsync();

        var result = await service.SearchAsync("hInO");

        Assert.That(result.Single().Id, Is.EqualTo(hino.Id));
    }
}
