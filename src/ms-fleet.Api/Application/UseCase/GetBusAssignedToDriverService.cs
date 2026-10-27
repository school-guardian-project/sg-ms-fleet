using ms_fleet.Api.Application.Dto.Response;
using ms_fleet.Api.Application.Mapper;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

/// <summary>
/// Bus activo asignado a un conductor, sin reglas de quien pregunta. Lo consume el
/// gRPC interno; los clientes REST pasan por <see cref="AuthorizedGetBusAssignedToDriverService"/>.
/// </summary>
public class GetBusAssignedToDriverService : IFindBusAssignedToDriverQuery
{
    private readonly IDriverAssignmentRepository _assignmentRepository;
    private readonly IBusRepository _busRepository;
    private readonly IBusMapper _busMapper;

    public GetBusAssignedToDriverService(
        IDriverAssignmentRepository assignmentRepository,
        IBusRepository busRepository,
        IBusMapper busMapper)
    {
        _assignmentRepository = assignmentRepository;
        _busRepository = busRepository;
        _busMapper = busMapper;
    }

    public async Task<BusAssignedResponse?> ExecuteAsync(Guid profileId, CancellationToken ct = default)
    {
        var assignment = await _assignmentRepository.GetActiveByProfileIdAsync(profileId, ct);
        if (assignment is null) return null;

        var bus = await _busRepository.GetByIdAsync(assignment.BusId, ct);
        if (bus is null || bus.Status != Status.Active) return null;

        return _busMapper.ToAssigned(bus);
    }
}
