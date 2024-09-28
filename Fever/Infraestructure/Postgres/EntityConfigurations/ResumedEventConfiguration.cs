using System.Reflection.Emit;
using Fever.Domain.Features.Events.Entities;
using Fever.Domain.Features.GetEvents.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Fever.Infraestructure.Postgres.EntityConfigurations;

public class ResumedEventConfiguration : IEntityTypeConfiguration<ResumedEvent>
{
    public void Configure(EntityTypeBuilder<ResumedEvent> builder)
    {
        builder.ToTable("resumed_events");

        builder.Property(re => re.EventId).HasColumnName("event_id");

        builder.Property(re => re.Title).HasColumnType("varchar(100)").HasColumnName("title");

        builder.Property(re => re.StartDate).HasColumnType("timestamp").HasColumnName("start_date");

        builder.Property(re => re.EndDate).HasColumnType("timestamp").HasColumnName("end_date");

        builder.Property(re => re.MinPrice).HasColumnType("decimal").HasColumnName("min_price");

        builder.Property(re => re.MaxPrice).HasColumnType("decimal").HasColumnName("max_price");

        builder.HasKey(re => re.EventId);

        builder
            .HasOne(re => re.Event)
            .WithOne(e => e.ResumedEvent)
            .HasForeignKey<ResumedEvent>(re => re.EventId);

        // Indexes
        builder.HasIndex(e => e.StartDate).HasDatabaseName("idx_resume_event_start_date");

        builder.HasIndex(e => e.EndDate).HasDatabaseName("idx_resume_event_end_date");
    }
}
