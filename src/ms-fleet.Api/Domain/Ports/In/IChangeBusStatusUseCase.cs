using ms_fleet.Api.Application.Dto.Request;

namespace ms_fleet.Api.Domain.Ports.In;

public interface IChangeBusStatusUseCase
{
    Task ExecuteAsync(Guid busId, ChangeStatusRequest request, CancellationToken ct = default);
}