using Grpc.Core;
using ms_fleet.Api.Domain.Ports.In;

namespace ms_fleet.Api.Infrastructure.Grpc;

/// <summary>
/// gRPC server for route.v1.RouteService: the bus lookups ms-route needs for
/// the guardian (FR-9.1) and driver (FR-9.2) route views. Inter-service traffic
/// is gRPC only (ADR-005) and is authenticated by
/// <see cref="InternalApiKeyInterceptor"/>.
/// </summary>
public class RouteGrpcService : RouteService.RouteServiceBase
{
    private readonly IFindBusAssignedToDriverQuery _getBusAssignedToDriver;
    private readonly IGetBusDetailUseCase _getBusDetail;

    public RouteGrpcService(
        IFindBusAssignedToDriverQuery getBusAssignedToDriver,
        IGetBusDetailUseCase getBusDetail)
    {
        _getBusAssignedToDriver = getBusAssignedToDriver;
        _getBusDetail = getBusDetail;
    }

    public override async Task<BusAssignedResponse> GetBusAssignedToDriver(
        GetBusAssignedToDriverRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProfileId, out var profileId))
        {
            throw new RpcException(new Status(
                StatusCode.InvalidArgument, "profile_id must be a GUID"));
        }

        var bus = await _getBusAssignedToDriver.ExecuteAsync(profileId, context.CancellationToken);

        // Paridad con el contrato REST previo: sin bus asignado => no asignado.
        if (bus is null)
        {
            throw new RpcException(new Status(
                StatusCode.NotFound, "No bus assigned to driver"));
        }

        return new BusAssignedResponse
        {
            BusId = bus.Id.ToString(),
            Plate = bus.Plate
        };
    }

    public override async Task<BusDetailsResponse> GetBusDetails(
        GetBusDetailsRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.BusId, out var busId))
        {
            throw new RpcException(new Status(
                StatusCode.InvalidArgument, "bus_id must be a GUID"));
        }

        var bus = await _getBusDetail.ExecuteAsync(busId, context.CancellationToken);
        if (bus is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Bus not found"));
        }

        return new BusDetailsResponse
        {
            BusId = bus.Id.ToString(),
            Plate = bus.Plate,
            Capacity = bus.Capacity,
            DriverProfileId = bus.DriverProfileId?.ToString() ?? string.Empty,
            DriverName = bus.DriverName ?? string.Empty
        };
    }
}
