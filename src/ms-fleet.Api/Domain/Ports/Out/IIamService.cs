namespace ms_fleet.Api.Domain.Ports.Out;

public interface IIamService
{
    Task<bool> ProfileExistsAsync(Guid profileId, CancellationToken ct = default);
}
