namespace ms_fleet.Api.Domain.Ports.In;

public interface IListBusesUseCase
{
    Task<IReadOnlyList<Bus>> ExecuteAsync(CancellationToken ct = default);
}
