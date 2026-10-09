using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Tests.Fakes;

public class InMemoryCampusReferenceRepository : ICampusReferenceRepository
{
    public readonly Dictionary<Guid, List<Guid>> CampusIdsBySchool = new();

    public Task<IReadOnlyCollection<Guid>> GetCampusIdsBySchoolAsync(Guid schoolId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyCollection<Guid>>(
            CampusIdsBySchool.TryGetValue(schoolId, out var ids) ? ids.ToList() : new List<Guid>());
}
