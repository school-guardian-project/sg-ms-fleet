namespace ms_fleet.Api.Domain.Ports.In;

public interface IChangeBusStatusUseCase
{
    Task ExecuteAsync(Guid busId, Status status, CancellationToken ct = default);
}
