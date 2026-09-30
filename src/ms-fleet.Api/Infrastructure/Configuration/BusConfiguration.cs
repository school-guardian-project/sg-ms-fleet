using ms_fleet.Api.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ms_fleet.Api.Infrastructure.Persistence;

namespace ms_fleet.Api.Infrastructure.Configuration;

public class BusConfiguration : IEntityTypeConfiguration<BusEntity>
{
    public void Configure(EntityTypeBuilder<BusEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Plate).IsRequired().HasMaxLength(15);
        builder.Property(x => x.Capacity).IsRequired();
        builder.Property(x => x.ModelId).IsRequired().HasColumnType("tinyint");
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.Plate).IsUnique();
    }
}
