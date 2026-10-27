using ms_fleet.Api.Application.Dto.Request;

namespace ms_fleet.Api.Domain.Ports.In;

public interface IUpdateBusUseCase
{
    Task ExecuteAsync(Guid busId, UpdateBusRequest request, CancellationToken ct = default);
}