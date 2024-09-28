using Fever.Domain.Features.Events.Entities;
using Fever.Presentation.Features.Events.DTOs;
using Fever.Presentation.Features.Events.Extensions;

namespace Fever.Infraestructure.EventsProviders.Mappers;

public static class BaseEventListMapper
{
    public static BaseEvent? CreateBaseEvent(BaseEventDTO baseEventDto, ILogger logger)
    {
        if (!baseEventDto.IsValid())
        {
            return null;
        }

        var eventList = new List<Event>();

        foreach (var eventEntity in baseEventDto.Events)
        {
            var newEvent = CreateEvent(eventEntity, baseEventDto.BaseEventId, logger);

            if (newEvent is not null)
            {
                eventList.Add(newEvent);
            }
        }

        var baseEvent = new BaseEvent
        {
            BaseEventId = baseEventDto.BaseEventId,
            SellMode = baseEventDto.SellMode,
            Title = baseEventDto.Title,
            Events = eventList,
        };
        return baseEvent;
    }

    public static Event? CreateEvent(EventDTO eventDto, int baseEventId, ILogger logger)
    {
        var eventStartDate = ParseDateTime(eventDto.EventStartDate, logger);
        var eventEndDate = ParseDateTime(eventDto.EventEndDate, logger);
        var sellFrom = ParseDateTime(eventDto.SellFrom, logger);
        var sellTo = ParseDateTime(eventDto.SellTo, logger);

        // If any date is null, return null to exclude the event from creation
        if (eventStartDate == null || eventEndDate == null || sellFrom == null || sellTo == null)
        {
            return null;
        }

        return new Event
        {
            Id = Guid.NewGuid(),
            EventId = eventDto.EventId,
            BaseEventId = baseEventId,
            EventStartDate = (DateTime)eventStartDate,
            EventEndDate = (DateTime)eventEndDate,
            SellFrom = (DateTime)sellFrom,
            SellTo = (DateTime)sellTo,
            SoldOut = eventDto.SoldOut,
            Zones = eventDto.Zones.Select(CreateZone).ToList(),
        };
    }

    public static Zone CreateZone(ZoneDTO zoneDto)
    {
        return new Zone
        {
            Id = Guid.NewGuid(),
            ZoneId = zoneDto.ZoneId,
            Capacity = zoneDto.Capacity,
            Price = zoneDto.Price,
            Name = zoneDto.Name,
            Numbered = zoneDto.Numbered,
        };
    }

    public static DateTime? ParseDateTime(string dateTimeString, ILogger logger)
    {
        if (DateTime.TryParse(dateTimeString, out var parsedDateTime))
        {
            return parsedDateTime;
        }
        else
        {
            logger.LogWarning($"Incorrect datetime value: {dateTimeString}");
            return null;
        }
    }
}
