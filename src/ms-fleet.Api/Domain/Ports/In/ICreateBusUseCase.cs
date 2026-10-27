using ms_fleet.Api.Application.Dto.Request;
using ms_fleet.Api.Application.Dto.Response;

namespace ms_fleet.Api.Domain.Ports.In;

public interface ICreateBusUseCase
{
    Task<CreateBusResponse> ExecuteAsync(CreateBusRequest request, CancellationToken ct = default);
}