using Fever.Application.Interfaces;
using MediatR;

namespace Fever.Application.Features.Events.Commands.UpdateEvents;

public sealed class UpdateEventsCommandHandler : IRequestHandler<UpdateEventsCommand>
{
    private readonly IExternalEventService _externalEventService;
    private readonly IEventsRepository _eventRespository;

    public UpdateEventsCommandHandler(
        IExternalEventService externalEventService,
        IEventsRepository eventsRespository
    )
    {
        _externalEventService = externalEventService;
        _eventRespository = eventsRespository;
    }

    public async Task Handle(UpdateEventsCommand command, CancellationToken cancellationToken)
    {
        var baseEventList = await _externalEventService.FetchEventsAsync(cancellationToken);

        await _eventRespository.SaveUpdatedEvents(baseEventList, cancellationToken);
    }
}
