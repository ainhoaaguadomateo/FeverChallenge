using System.Data;
using Fever.Application.Interfaces;
using Fever.Infraestructure.Postgres;
using Fever.Infraestructure.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Fever.Infraestructure.BackgroundServices.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<UpdateEventsBackgroundService>();

        return services;
    }
}
