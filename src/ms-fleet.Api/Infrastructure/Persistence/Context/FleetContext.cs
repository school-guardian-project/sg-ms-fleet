using ms_fleet.Api.Infrastructure.Persistence.Entity;
using ms_fleet.Api.Domain.Model;
using Microsoft.EntityFrameworkCore;
using ms_fleet.Api.Infrastructure.Configuration;
using ms_fleet.Api.Infrastructure.Persistence;

namespace ms_fleet.Api.Infrastructure.Persistence.Context;

public class FleetContext : DbContext
{
    public FleetContext(DbContextOptions options) : base(options) { }

    public DbSet<BusEntity> Buses => Set<BusEntity>();
    public DbSet<DriverAssignmentEntity> DriverAssignments => Set<DriverAssignmentEntity>();
    public DbSet<BrandEntity> Brands => Set<BrandEntity>();
    public DbSet<ModelEntity> Models => Set<ModelEntity>();

    // Referencia cross-schema (solo lectura) a School.SchoolCampus para el filtrado multi-tenant.
    public DbSet<SchoolCampusEntity> SchoolCampuses => Set<SchoolCampusEntity>();
    public DbSet<GpsDeviceEntity> GpsDevices => Set<GpsDeviceEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("Fleet");

        modelBuilder.Entity<BusEntity>().ToTable("Bus", schema: "Fleet");
        modelBuilder.Entity<DriverAssignmentEntity>().ToTable("DriverAssignemts", schema: "Fleet");
        modelBuilder.Entity<BrandEntity>().ToTable("Brand", schema: "Fleet");
        modelBuilder.Entity<ModelEntity>().ToTable("Model", schema: "Fleet");

        modelBuilder.Entity<SchoolCampusEntity>(entity =>
        {
            entity.ToTable("SchoolCampus", schema: "School");
            entity.HasKey(x => x.Id);
        });

        modelBuilder.Entity<GpsDeviceEntity>(entity =>
        {
            entity.ToTable("GpsDevice", schema: "Gps", t => t.ExcludeFromMigrations());
            entity.HasKey(x => x.Id);
        });

        modelBuilder.ApplyConfiguration(new BusConfiguration());
        modelBuilder.ApplyConfiguration(new DriverAssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new BrandConfiguration());
        modelBuilder.ApplyConfiguration(new ModelConfiguration());
    }

    protected FleetContext() { }
}
