using Fever.Application.Interfaces;
using Fever.Domain.Features.Events.Entities;
using Fever.Infraestructure.EventsProviders.Mappers;
using Fever.Infraestructure.Postgres.Repositories;
using Fever.Presentation.Features.Events.DTOs;

namespace Fever.Infraestructure.EventsProviders.UpdateEvents.Services;

public class FetchEventsService : IFetchEventsService
{
    private ILogger<EventsRepository> _logger;

    public FetchEventsService(ILogger<EventsRepository> logger)
    {
        _logger = logger;
    }

    public List<BaseEvent> GetBaseEventList(EventListDTO eventListDto)
    {
        var baseEventList = new List<BaseEvent>();

        foreach (var baseEvent in eventListDto.Output.BaseEvents)
        {
            var newBaseEvent = BaseEventListMapper.CreateBaseEvent(baseEvent, _logger);

            if (newBaseEvent is not null)
            {
                baseEventList.Add(newBaseEvent);
            }
        }

        return baseEventList;
    }
}
