using Fever.Application.Interfaces;
using Fever.Infraestructure.EventsProviders.UpdateEvents.Services;

namespace Fever.Infraestructure.EventsProviders.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddProviderServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        //Infraestructure
        services.AddHttpClient<ExternalEventService>();

        services.AddScoped<IExternalEventService, ExternalEventService>();

        services.AddScoped<IFetchEventsService, FetchEventsService>();

        return services;
    }
}
