using ms_fleet.Api.Domain.Model;
using ms_fleet.Api.Domain.Ports.Out;

namespace ms_fleet.Api.Application.Security;

/// <summary>
/// Sedes visibles para el tenant del token actual. null = sin filtro
/// (SuperAdmin, o llamada interna sin usuario).
/// Admin (roleId 1): todas las sedes de su colegio. Student/Driver/Parent (2/3/4): solo su sede.
/// Un tenant filtrable sin colegio ni sede resolubles no ve ninguna (falla cerrado).
/// </summary>
public sealed class BusTenantScope
{
    private readonly ITenantProvider _tenantProvider;
    private readonly ICampusReferenceRepository _campusReferenceRepository;

    public BusTenantScope(ITenantProvider tenantProvider, ICampusReferenceRepository campusReferenceRepository)
    {
        _tenantProvider = tenantProvider;
        _campusReferenceRepository = campusReferenceRepository;
    }

    public async Task<IReadOnlyCollection<Guid>?> GetVisibleCampusIdsAsync(CancellationToken ct = default)
    {
        if (!_tenantProvider.ShouldFilter) return null;

        if (_tenantProvider.RoleId == FleetRoles.Admin && _tenantProvider.SchoolId is { } schoolId)
            return await _campusReferenceRepository.GetCampusIdsBySchoolAsync(schoolId, ct);

        if (_tenantProvider.RoleId != FleetRoles.Admin && _tenantProvider.CampusId is { } campusId)
            return [campusId];

        return [];
    }

    public async Task<bool> CanSeeCampusAsync(Guid campusId, CancellationToken ct = default)
    {
        var visible = await GetVisibleCampusIdsAsync(ct);
        return visible is null || visible.Contains(campusId);
    }
}
