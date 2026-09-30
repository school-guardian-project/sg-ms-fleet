namespace ms_fleet.Api.Domain.Ports.In;

public interface IDeleteBusUseCase
{
    Task ExecuteAsync(Guid busId, CancellationToken ct = default);
}
