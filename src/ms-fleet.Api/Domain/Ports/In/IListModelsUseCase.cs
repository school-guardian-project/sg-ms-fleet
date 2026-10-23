namespace ms_fleet.Api.Domain.Ports.In;

public interface IListModelsUseCase
{
    Task<List<Application.Dtos.ModelListDto>> ExecuteAsync(byte? brandId, CancellationToken ct);
}
