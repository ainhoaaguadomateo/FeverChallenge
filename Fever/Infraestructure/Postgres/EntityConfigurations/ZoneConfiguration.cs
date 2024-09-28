using Fever.Domain.Features.Events.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fever.Infraestructure.Postgres.EntityConfigurations;

public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.ToTable("zones");

        builder.Property(z => z.ZoneId).HasColumnType("int").HasColumnName("zone_id");

        builder.Property(z => z.Capacity).HasColumnType("int").HasColumnName("capacity");

        builder.Property(z => z.Price).HasColumnType("decimal(18, 2)").HasColumnName("price");

        builder
            .Property(z => z.Name)
            .IsRequired()
            .HasColumnType("varchar(100)")
            .HasColumnName("name");

        builder.Property(z => z.Numbered).HasColumnType("boolean").HasColumnName("numbered");

        builder.Property(z => z.EventId).HasColumnName("event_id");

        builder.HasKey(z => z.Id);

        builder
            .HasIndex(z => new { z.ZoneId, z.EventId })
            .IsUnique()
            .HasDatabaseName("ux_zone_zone_id_event_id");

        // Foreign Key
        builder.HasOne(z => z.Event).WithMany(e => e.Zones).HasForeignKey(z => z.EventId);
    }
}
