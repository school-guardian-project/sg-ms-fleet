using ms_fleet.Api.Domain.Model;
namespace ms_fleet.Api.Domain.Ports.In;

public interface IUpdateBusUseCase
{
    /// <summary>
    /// Los parametros opcionales (placa, GPS y estado del GPS) solo se aplican
    /// cuando llegan; asi un cliente viejo que no los envia no los borra.
    /// </summary>
    Task ExecuteAsync(
        Guid busId,
        Guid campuseId,
        DateTime soatValidity,
        byte capacity,
        int modelId,
        string? plate = null,
        Guid? gpsDeviceId = null,
        bool? gpsStatus = null,
        string? gpsImei = null,
        CancellationToken ct = default);
}
