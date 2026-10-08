namespace ms_fleet.Api.Domain.Ports.Out;

/// <summary>
/// Referencia de solo lectura a School.SchoolCampus (propiedad de ms-school-management).
/// Se usa para resolver las sedes de un colegio al filtrar buses por tenant.
/// </summary>
public interface ICampusReferenceRepository
{
    Task<IReadOnlyCollection<Guid>> GetCampusIdsBySchoolAsync(Guid schoolId, CancellationToken ct = default);
}
