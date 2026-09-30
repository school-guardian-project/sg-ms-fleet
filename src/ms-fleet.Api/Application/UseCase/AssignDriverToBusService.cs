using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.UseCase;

public class AssignDriverToBusService : IAssignDriverToBusUseCase
{
    private readonly IBusRepository _busRepository;
    private readonly IDriverAssignmentRepository _assignmentRepository;
    private readonly IIamService _iamService;

    public AssignDriverToBusService(
        IBusRepository busRepository,
        IDriverAssignmentRepository assignmentRepository,
        IIamService iamService)
    {
        _busRepository = busRepository;
        _assignmentRepository = assignmentRepository;
        _iamService = iamService;
    }

    public async Task ExecuteAsync(Guid busId, Guid profileId, CancellationToken ct = default)
    {
        var bus = await _busRepository.GetByIdAsync(busId, ct)
            ?? throw new InvalidOperationException("Bus not found");

        if (bus.Status != Status.Active)
            throw new InvalidOperationException("Cannot assign a driver to an inactive bus");

        if (!await _iamService.ProfileExistsAsync(profileId, ct))
            throw new InvalidOperationException("Driver profile not found");

        var existingAssignment = await _assignmentRepository.GetActiveByProfileIdAsync(profileId, ct);
        if (existingAssignment != null)
            throw new InvalidOperationException("Driver already assigned to a bus");

        var assignment = new DriverAssignment
        {
            ProfileId = profileId,
            BusId = busId
        };

        await _assignmentRepository.AddAsync(assignment, ct);
    }
}
