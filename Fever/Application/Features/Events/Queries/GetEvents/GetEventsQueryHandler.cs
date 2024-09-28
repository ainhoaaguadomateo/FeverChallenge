using Fever.Application.Interfaces;
using Fever.Domain.Features.GetEvents.Models;
using MediatR;

namespace Fever.Application.Features.Events.Queries.GetEvents;

public sealed class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, GetEventResponseDto>
{
    private readonly IEventsRepository _eventsRepository;

    public GetEventsQueryHandler(IEventsRepository eventsRepository)
    {
        _eventsRepository = eventsRepository;
    }

    public async Task<GetEventResponseDto> Handle(
        GetEventsQuery query,
        CancellationToken cancellationToken
    )
    {
        var response = await _eventsRepository.GetEventsAsync(
            query.StartsAt,
            query.EndsAt,
            cancellationToken
        );

        return response;
    }
}
