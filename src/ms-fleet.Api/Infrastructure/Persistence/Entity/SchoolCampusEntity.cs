namespace ms_fleet.Api.Infrastructure.Persistence.Entity;

/// <summary>
/// Entidad de referencia (solo lectura) sobre la tabla School.SchoolCampus.
/// ms-fleet únicamente necesita Id y SchoolId para resolver las sedes de un colegio;
/// el dueño del esquema sigue siendo ms-school-management.
/// </summary>
public class SchoolCampusEntity
{
    public Guid Id { get; set; }
    public Guid SchoolId { get; set; }
}
