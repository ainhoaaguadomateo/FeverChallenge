using Fever.Application.Features.Events.Queries.GetEvents;
using FluentValidation;

namespace Fever.Application.Features.Events.Validators;

public class GetEventsQueryValidator : AbstractValidator<GetEventsQuery>
{
    public GetEventsQueryValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                !x.StartsAt.HasValue || !x.EndsAt.HasValue || x.StartsAt.Value <= x.EndsAt.Value
            )
            .WithErrorCode("IncorrectDateRange")
            .WithMessage("The start date must be before the end date.");
    }
}
