namespace ms_fleet.Api.Domain.Ports.In;

public interface IUnassignDriverFromBusUseCase
{
    Task ExecuteAsync(Guid busId, CancellationToken ct = default);
}
