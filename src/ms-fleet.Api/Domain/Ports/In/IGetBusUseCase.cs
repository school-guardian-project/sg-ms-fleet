namespace ms_fleet.Api.Domain.Ports.In;

public interface IGetBusUseCase
{
    Task<Bus?> ExecuteAsync(Guid busId, CancellationToken ct = default);
}
