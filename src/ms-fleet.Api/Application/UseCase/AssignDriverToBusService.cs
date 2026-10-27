using ms_fleet.Api.Application.Dto.Request;
using ms_fleet.Api.Domain.Ports.In;
using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

using ms_fleet.Api.Application.Security;

namespace ms_fleet.Api.Application.UseCase;

public class AssignDriverToBusService : IAssignDriverToBusUseCase
{
    private readonly IBusRepository _busRepository;
    private readonly IDriverAssignmentRepository _assignmentRepository;
    private readonly IIamService _iamService;
    private readonly BusAccessPolicy _accessPolicy;

    public AssignDriverToBusService(
        IBusRepository busRepository,
        IDriverAssignmentRepository assignmentRepository,
        IIamService iamService,
        BusAccessPolicy accessPolicy)
    {
        _busRepository = busRepository;
        _assignmentRepository = assignmentRepository;
        _iamService = iamService;
        _accessPolicy = accessPolicy;
    }

    public async Task ExecuteAsync(Guid busId, AssignDriverRequest request, CancellationToken ct = default)
    {
        var bus = await _busRepository.GetByIdAsync(busId, ct)
            ?? throw new InvalidOperationException("Bus not found");

        await _accessPolicy.EnsureCanManageBusAsync(bus, ct);

        if (bus.Status != Status.Active)
            throw new InvalidOperationException("Cannot assign a driver to an inactive bus");

        var busHasDriver = await _assignmentRepository.GetActiveByBusIdAsync(busId, ct);
        if (busHasDriver != null)
            throw new InvalidOperationException("The bus already has a driver assigned");

        if (!await _iamService.ProfileExistsAsync(request.ProfileId, ct))
            throw new InvalidOperationException("Driver profile not found");

        var existingAssignment = await _assignmentRepository.GetActiveByProfileIdAsync(request.ProfileId, ct);
        if (existingAssignment != null)
            throw new InvalidOperationException("Driver already assigned to a bus");

        var assignment = new DriverAssignment
        {
            ProfileId = request.ProfileId,
            BusId = busId
        };

        await _assignmentRepository.AddAsync(assignment, ct);
    }
}