using Fever.Application.Features.Events.Queries.GetEvents;
using Fever.Application.Features.Events.Validators;
using FluentValidation;

namespace Fever.Application.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<GetEventsQuery>, GetEventsQueryValidator>();

        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        return services;
    }
}
