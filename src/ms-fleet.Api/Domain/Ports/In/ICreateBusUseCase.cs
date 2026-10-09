using ms_fleet.Api.Domain.Model;
namespace ms_fleet.Api.Domain.Ports.In;

public interface ICreateBusUseCase
{
    Task<Guid> ExecuteAsync(
        Guid campuseId,
        DateTime soatValidity,
        Guid gpsDeviceId,
        byte capacity,
        string plate,
        int modelId,
        bool? gpsStatus = null,
        string? gpsImei = null,
        CancellationToken ct = default);
}
