using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using ms_fleet.Api.Application.Dtos;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Infrastructure.Grpc;
using NUnit.Framework;

namespace ms_fleet.Api.Tests.Infrastructure;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class RouteGrpcServiceTests
{
    private static readonly Guid ProfileId = Guid.NewGuid();
    private static readonly Guid BusId = Guid.NewGuid();

    private sealed class StubBusAssigned : IGetBusAssignedToDriverUseCase
    {
        public BusAssignedDto? Result { get; set; }

        public Task<BusAssignedDto?> ExecuteAsync(Guid profileId, CancellationToken ct = default)
            => Task.FromResult(Result);
    }

    private sealed class StubBusDetail : IGetBusDetailUseCase
    {
        public BusDetailDto? Result { get; set; }

        public Task<BusDetailDto?> ExecuteAsync(Guid busId, CancellationToken ct = default)
            => Task.FromResult(Result);
    }

    private readonly StubBusAssigned _assigned = new();
    private readonly StubBusDetail _detail = new();

    private RouteGrpcService CreateService() => new(_assigned, _detail);

    private static TestServerCallContext Context() => new();

    [Test]
    public async Task GetBusAssignedToDriver_WithAssignment_ReturnsBusIdAndPlate()
    {
        _assigned.Result = new BusAssignedDto(BusId, "ABC123", Guid.NewGuid());

        var result = await CreateService().GetBusAssignedToDriver(
            new GetBusAssignedToDriverRequest { ProfileId = ProfileId.ToString() }, Context());

        Assert.That(result.BusId, Is.EqualTo(BusId.ToString()));
        Assert.That(result.Plate, Is.EqualTo("ABC123"));
    }

    [Test]
    public void GetBusAssignedToDriver_WithoutAssignment_ThrowsNotFound()
    {
        _assigned.Result = null;

        var ex = Assert.ThrowsAsync<RpcException>(async () =>
            await CreateService().GetBusAssignedToDriver(
                new GetBusAssignedToDriverRequest { ProfileId = ProfileId.ToString() }, Context()));

        Assert.That(ex!.StatusCode, Is.EqualTo(StatusCode.NotFound));
    }

    [Test]
    public void GetBusAssignedToDriver_InvalidProfileId_ThrowsInvalidArgument()
    {
        var ex = Assert.ThrowsAsync<RpcException>(async () =>
            await CreateService().GetBusAssignedToDriver(
                new GetBusAssignedToDriverRequest { ProfileId = "not-a-guid" }, Context()));

        Assert.That(ex!.StatusCode, Is.EqualTo(StatusCode.InvalidArgument));
    }

    [Test]
    public async Task GetBusDetails_WithBus_ReturnsPlateCapacityAndDriver()
    {
        _detail.Result = new BusDetailDto(
            BusId, "ABC123", Guid.NewGuid(), DateTime.UtcNow.AddYears(1), Guid.NewGuid(),
            Capacity: 40, ModelId: 1, Status: "Active", DriverName: "Juan Perez",
            Brand: "Marc", Model: "Urbano", GpsImei: "123456789", GpsStatus: true,
            DriverProfileId: ProfileId);

        var result = await CreateService().GetBusDetails(
            new GetBusDetailsRequest { BusId = BusId.ToString() }, Context());

        Assert.That(result.BusId, Is.EqualTo(BusId.ToString()));
        Assert.That(result.Plate, Is.EqualTo("ABC123"));
        Assert.That(result.Capacity, Is.EqualTo(40));
        Assert.That(result.DriverName, Is.EqualTo("Juan Perez"));
        Assert.That(result.DriverProfileId, Is.EqualTo(ProfileId.ToString()));
    }

    [Test]
    public async Task GetBusDetails_WithoutDriver_LeavesDriverProfileEmpty()
    {
        _detail.Result = new BusDetailDto(
            BusId, "ABC123", Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(),
            Capacity: 40, ModelId: 1, Status: "Active", DriverName: string.Empty,
            Brand: string.Empty, Model: string.Empty);

        var result = await CreateService().GetBusDetails(
            new GetBusDetailsRequest { BusId = BusId.ToString() }, Context());

        Assert.That(result.DriverProfileId, Is.Empty);
        Assert.That(result.DriverName, Is.Empty);
    }

    [Test]
    public void GetBusDetails_UnknownBus_ThrowsNotFound()
    {
        _detail.Result = null;

        var ex = Assert.ThrowsAsync<RpcException>(async () =>
            await CreateService().GetBusDetails(
                new GetBusDetailsRequest { BusId = BusId.ToString() }, Context()));

        Assert.That(ex!.StatusCode, Is.EqualTo(StatusCode.NotFound));
    }

    [Test]
    public void GetBusDetails_InvalidBusId_ThrowsInvalidArgument()
    {
        var ex = Assert.ThrowsAsync<RpcException>(async () =>
            await CreateService().GetBusDetails(
                new GetBusDetailsRequest { BusId = string.Empty }, Context()));

        Assert.That(ex!.StatusCode, Is.EqualTo(StatusCode.InvalidArgument));
    }
}
