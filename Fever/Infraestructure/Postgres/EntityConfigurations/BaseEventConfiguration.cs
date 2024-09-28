using Fever.Domain.Features.Events.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fever.Infraestructure.Postgres.EntityConfigurations;

public class BaseEventConfiguration : IEntityTypeConfiguration<BaseEvent>
{
    public void Configure(EntityTypeBuilder<BaseEvent> builder)
    {
        builder.ToTable("base_events");

        builder
            .Property(s => s.BaseEventId)
            .HasColumnType("int")
            .HasColumnName("base_event_id")
            .IsRequired();

        builder
            .Property(s => s.SellMode)
            .HasColumnType("varchar(50)")
            .HasMaxLength(50)
            .HasColumnName("sell_mode")
            .IsRequired(true);

        builder
            .Property(s => s.Title)
            .HasColumnType("varchar(50)")
            .HasColumnName("title")
            .IsRequired();

        builder.HasKey(e => e.BaseEventId);

        builder
            .HasMany(be => be.Events)
            .WithOne(e => e.BaseEvent)
            .HasForeignKey(e => e.BaseEventId);

        //Indexes
        builder.HasIndex(be => be.BaseEventId).HasDatabaseName("idx_base_events_base_event_id");
    }
}
