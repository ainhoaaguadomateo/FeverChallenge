using Fever.Domain.Features.Events.Entities;
using Fever.Domain.Features.GetEvents.Entities;
using Fever.Infraestructure.Postgres.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Fever.Infraestructure.Postgres;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Event> Events { get; set; }

    public DbSet<BaseEvent> BaseEvents { get; set; }

    public DbSet<Zone> Zones { get; set; }

    public DbSet<ResumedEvent> ResumedEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new BaseEventConfiguration());
        modelBuilder.ApplyConfiguration(new ZoneConfiguration());
        modelBuilder.ApplyConfiguration(new ResumedEventConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
