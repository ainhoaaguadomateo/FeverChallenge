using Fever.Application.Features.Events.Queries.GetEvents;
using Fever.Domain.Features.GetEvents.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fever.Presentation.Features.Events.Endpoints;

public static class EventEndpoints
{
    public static WebApplication AddEventsEndpoints(this WebApplication app)
    {
        app.MapGet("/search", Search).WithName("Search").WithOpenApi();

        return app;
    }

    public static async Task<IResult> Search(
        [AsParameters] GetEventsQuery query,
        [FromServices] IMediator mediator,
        [FromServices] IValidator<GetEventsQuery> validator,
        CancellationToken cancellationToken
    )
    {
        var validationResult = validator.Validate(query);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var response = await mediator.Send(query, cancellationToken);

        return Results.Json(ApiResponse<object>.Success(response));
    }
}
