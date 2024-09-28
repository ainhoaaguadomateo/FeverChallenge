using System.Reflection.Emit;
using Fever.Domain.Features.Events.Entities;
using Fever.Domain.Features.GetEvents.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fever.Infraestructure.Postgres.EntityConfigurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.EventId).HasColumnType("int").HasColumnName("event_id");

        builder
            .Property(e => e.BaseEventId)
            .IsRequired()
            .HasColumnType("int")
            .HasColumnName("base_event_id");

        builder
            .Property(e => e.EventStartDate)
            .HasColumnType("timestamp")
            .HasColumnName("event_start_date");

        builder
            .Property(e => e.EventEndDate)
            .HasColumnType("timestamp")
            .HasColumnName("event_end_date");

        builder.Property(e => e.SellFrom).HasColumnType("timestamp").HasColumnName("sell_from");

        builder.Property(e => e.SellTo).HasColumnType("timestamp").HasColumnName("sell_to");

        builder.Property(e => e.SoldOut).HasColumnType("boolean").HasColumnName("sold_out");

        builder.HasKey(e => e.Id);

        builder
            .HasOne(e => e.BaseEvent)
            .WithMany(be => be.Events)
            .HasForeignKey(e => e.BaseEventId);

        builder.HasMany(e => e.Zones).WithOne(z => z.Event).HasForeignKey(z => z.EventId);

        builder
            .HasOne(e => e.ResumedEvent)
            .WithOne(re => re.Event)
            .HasForeignKey<ResumedEvent>(re => re.EventId);

        builder
            .HasIndex(e => new { e.EventId, e.BaseEventId })
            .IsUnique()
            .HasDatabaseName("idx_event_baseevent_event_id_base_event_id");

        // Indexes
        builder.HasIndex(e => e.EventStartDate).HasDatabaseName("idx_event_event_start_date");

        builder.HasIndex(e => e.EventEndDate).HasDatabaseName("idx_event_event_end_date");

        builder.HasIndex(e => e.BaseEventId).HasDatabaseName("idx_event_base_event_id");
    }
}
