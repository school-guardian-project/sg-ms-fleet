using ms_fleet.Api.Domain.Model;
namespace ms_fleet.Api.Domain.Ports.In;

public interface IUpdateBusUseCase
{
    Task ExecuteAsync(
        Guid busId,
        Guid campuseId,
        DateTime soatValidity,
        byte capacity,
        int modelId,
        CancellationToken ct = default);
}
