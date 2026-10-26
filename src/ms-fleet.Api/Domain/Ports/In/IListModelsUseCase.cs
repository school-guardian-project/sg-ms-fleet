using ms_fleet.Api.Application.Dto.Response;

namespace ms_fleet.Api.Domain.Ports.In;

public interface IListModelsUseCase
{
    Task<List<ModelListResponse>> ExecuteAsync(byte? brandId, CancellationToken ct);
}