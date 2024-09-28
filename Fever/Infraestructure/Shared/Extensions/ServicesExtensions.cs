using Fever.Application.Extensions;
using Fever.Application.Features.Events.Queries.GetEvents;
using Fever.Application.Features.Events.Validators;
using Fever.Infraestructure.BackgroundServices.Extensions;
using Fever.Infraestructure.EventsProviders.Extensions;
using Fever.Infraestructure.Postgres.Extensions;
using FluentValidation;

namespace Fever.Infraestructure.Shared.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddBackgroundServices();

        services.AddProviderServices(configuration);

        services.AddApplicationServices();

        services.AddPostgresServices(configuration);

        return services;
    }
}
