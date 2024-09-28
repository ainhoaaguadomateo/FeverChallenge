using System.Data;
using Fever.Application.Interfaces;
using Fever.Infraestructure.DataAccess;
using Fever.Infraestructure.EventsProviders.UpdateEvents.Services;
using Fever.Infraestructure.Postgres;
using Fever.Infraestructure.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Fever.Infraestructure.Postgres.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddPostgresServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContextPool<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
        });

        services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(
            sp.GetRequiredService<IConfiguration>().GetConnectionString("PostgreSQL")
        ));

        services.AddScoped<IEventsRepository, EventsRepository>();

        services.AddScoped<IDapperService, DapperService>();

        return services;
    }
}
