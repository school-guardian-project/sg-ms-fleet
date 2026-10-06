using ms_fleet.Api.Domain.Model;
namespace ms_fleet.Api.Domain.Ports.In;

public interface IAssignDriverToBusUseCase
{
    Task ExecuteAsync(Guid busId, Guid profileId, CancellationToken ct = default);
}
