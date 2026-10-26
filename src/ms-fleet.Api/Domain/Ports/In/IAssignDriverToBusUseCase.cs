using ms_fleet.Api.Application.Dto.Request;

namespace ms_fleet.Api.Domain.Ports.In;

public interface IAssignDriverToBusUseCase
{
    Task ExecuteAsync(Guid busId, AssignDriverRequest request, CancellationToken ct = default);
}