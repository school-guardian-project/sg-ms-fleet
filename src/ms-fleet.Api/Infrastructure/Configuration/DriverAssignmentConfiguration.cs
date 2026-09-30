using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ms_fleet.Api.Infrastructure.Persistence;

namespace ms_fleet.Api.Infrastructure.Configuration;

public class DriverAssignmentConfiguration : IEntityTypeConfiguration<DriverAssignmentEntity>
{
    public void Configure(EntityTypeBuilder<DriverAssignmentEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProfileId).IsRequired();
        builder.Property(x => x.BusId).IsRequired();
        builder.Property(x => x.AssignedFrom).IsRequired();
        builder.HasIndex(x => x.ProfileId);
        builder.HasIndex(x => x.BusId);
    }
}
