using System.Data;
using System.Text;
using Fever.Application.Interfaces;
using Fever.Domain.Features.Events.Entities;
using Fever.Domain.Features.GetEvents.Entities;
using Fever.Domain.Features.GetEvents.Models;
using Fever.Infraestructure.EventsProviders.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Fever.Infraestructure.Postgres.Repositories;

public class EventsRepository : IEventsRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDapperService _dapperService;
    private readonly ILogger<EventsRepository> _logger;

    public EventsRepository(
        ApplicationDbContext dbContext,
        IDapperService dapperService,
        ILogger<EventsRepository> logger
    )
    {
        _dbContext = dbContext;
        _dapperService = dapperService;
        _logger = logger;
    }

    public async Task SaveUpdatedEvents(
        List<BaseEvent> baseEvents,
        CancellationToken cancellationToken
    )
    {
        foreach (var baseEvent in baseEvents)
        {
            var existingBaseEvent = await GetExistingBaseEvent(
                baseEvent.BaseEventId,
                cancellationToken
            );

            if (existingBaseEvent is null)
            {
                AddNewBaseEvent(baseEvent);
                continue;
            }

            UpdateExistingBaseEvent(baseEvent, existingBaseEvent);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<BaseEvent?> GetExistingBaseEvent(
        int baseEventId,
        CancellationToken cancellationToken
    )
    {
        return await _dbContext
            .BaseEvents.Include(be => be.Events)
            .ThenInclude(e => e.Zones)
            .FirstOrDefaultAsync(be => be.BaseEventId == baseEventId, cancellationToken)!;
    }

    private void AddNewBaseEvent(BaseEvent baseEvent)
    {
        _dbContext.BaseEvents.Add(baseEvent);

        if (baseEvent.SellMode == "online")
        {
            CreateResumedEvents(baseEvent);
        }
    }

    private void UpdateExistingBaseEvent(BaseEvent baseEvent, BaseEvent existingBaseEvent)
    {
        existingBaseEvent.Title = baseEvent.Title;
        existingBaseEvent.SellMode = baseEvent.SellMode;
        AddOrUpdateEvents(baseEvent, existingBaseEvent);
        RemoveOldZones(existingBaseEvent);
    }

    private void AddOrUpdateEvents(BaseEvent baseEvent, BaseEvent existingBaseEvent)
    {
        foreach (var eventEntity in baseEvent.Events)
        {
            var existingEvent = existingBaseEvent.Events.FirstOrDefault(e =>
                e.EventId == eventEntity.EventId && e.BaseEventId == eventEntity.BaseEventId
            );

            if (existingEvent == null)
            {
                AddNewEvent(baseEvent, eventEntity);
            }
            else
            {
                UpdateEvent(existingEvent, eventEntity);
                if (baseEvent.SellMode != "online")
                {
                    RemoveResumedEvent(eventEntity);
                }
                else
                {
                    var existingResumedEvent = _dbContext.ResumedEvents.FirstOrDefault(re =>
                        re.EventId == existingEvent.Id
                    );

                    if (existingResumedEvent is not null)
                    {
                        UpdateResumedEvent(existingResumedEvent, eventEntity, baseEvent.Title);
                    }
                }
            }
        }
    }

    private void RemoveOldZones(BaseEvent existingBaseEvent)
    {
        foreach (var existingEvent in existingBaseEvent.Events)
        {
            // Obtener las zonas que ya no están presentes en los datos actualizados
            var zonesToRemove = existingEvent
                .Zones.Where(ez =>
                    !existingBaseEvent.Events.Any(be => be.Zones.Any(z => z.ZoneId == ez.ZoneId))
                )
                .ToList();

            // Eliminar las zonas antiguas de la base de datos
            foreach (var zoneToRemove in zonesToRemove)
            {
                _dbContext.Zones.Remove(zoneToRemove);
            }
        }
    }

    private void RemoveResumedEvent(Event eventEntity)
    {
        var existingResumedEvent = _dbContext.ResumedEvents.FirstOrDefault(re =>
            re.EventId == eventEntity.Id
        );
        if (existingResumedEvent is not null)
        {
            _dbContext.ResumedEvents.Remove(existingResumedEvent);
        }
    }

    private void AddNewEvent(BaseEvent baseEvent, Event eventEntity)
    {
        _dbContext.Events.Add(eventEntity);

        if (baseEvent.SellMode == "online")
        {
            CreateResumedEvent(eventEntity, baseEvent.Title);
        }
    }

    private void UpdateEvent(Event existingEvent, Event eventEntity)
    {
        existingEvent.EventStartDate = eventEntity.EventStartDate;
        existingEvent.EventEndDate = eventEntity.EventEndDate;
        existingEvent.SellFrom = eventEntity.SellFrom;
        existingEvent.SellTo = eventEntity.SellTo;
        existingEvent.SoldOut = eventEntity.SoldOut;

        UpdateZones(existingEvent, eventEntity);
    }

    private void UpdateZones(Event existingEvent, Event eventEntity)
    {
        var zonesToRemove = existingEvent
            .Zones.Where(ez => !eventEntity.Zones.Any(z => z.ZoneId == ez.ZoneId))
            .ToList();

        foreach (var zoneToRemove in zonesToRemove)
        {
            _dbContext.Zones.Remove(zoneToRemove);
        }

        var zonesToAdd = eventEntity
            .Zones.Where(z => !existingEvent.Zones.Any(ez => z.ZoneId == ez.ZoneId))
            .ToList();

        foreach (var zoneToAdd in zonesToAdd)
        {
            _dbContext.Zones.Add(zoneToAdd);
        }

        foreach (var existingZone in existingEvent.Zones)
        {
            if (zonesToRemove.Any(z => z.ZoneId == existingZone.ZoneId))
                continue;

            var zoneEntity = eventEntity.Zones.FirstOrDefault(z => z.ZoneId == existingZone.ZoneId);

            if (zoneEntity != null)
            {
                UpdateZone(existingZone, zoneEntity);
            }
        }
    }

    private void UpdateZone(Zone existingZone, Zone zoneEntity)
    {
        existingZone.Capacity = zoneEntity.Capacity;
        existingZone.Price = zoneEntity.Price;
        existingZone.Name = zoneEntity.Name;
        existingZone.Numbered = zoneEntity.Numbered;
    }

    private void UpdateResumedEvent(
        ResumedEvent existentResumedEvent,
        Event eventItem,
        string title
    )
    {
        existentResumedEvent.Title = title;
        existentResumedEvent.StartDate = eventItem.EventStartDate;
        existentResumedEvent.EndDate = eventItem.EventEndDate;
        existentResumedEvent.MinPrice = eventItem.Zones.Min(e => e.Price);
        existentResumedEvent.MaxPrice = eventItem.Zones.Max(e => e.Price);
    }

    private void CreateResumedEvents(BaseEvent baseEvent)
    {
        var resumedEvents = baseEvent
            .Events.Select(eventItem => new ResumedEvent
            {
                EventId = eventItem.Id,
                Title = baseEvent.Title,
                StartDate = eventItem.EventStartDate,
                EndDate = eventItem.EventEndDate,
                MinPrice = eventItem.Zones.Any()
                    ? eventItem.Zones.Min(e => e.Price)
                    : (decimal?)null,
                MaxPrice = eventItem.Zones.Any()
                    ? eventItem.Zones.Max(e => e.Price)
                    : (decimal?)null,
            })
            .ToList();

        _dbContext.ResumedEvents.AddRange(resumedEvents);
    }

    private void CreateResumedEvent(Event eventItem, string title, Guid? id = null)
    {
        var newResumenEvent = new ResumedEvent
        {
            EventId = id ?? eventItem.Id,
            Title = title,
            StartDate = eventItem.EventStartDate,
            EndDate = eventItem.EventEndDate,
            MinPrice = eventItem.Zones.Min(e => e.Price),
            MaxPrice = eventItem.Zones.Max(e => e.Price),
        };

        _dbContext.ResumedEvents.Add(newResumenEvent);
    }

    public async Task<GetEventResponseDto> GetEventsAsync(
        DateTime? startsAt = null,
        DateTime? endsAt = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = GenerateQuery(startsAt, endsAt);

        try
        {
            var eventResults = await _dapperService.QueryAsync<GetEventListDTO>(
                query,
                new
                {
                    StartTime = startsAt ?? DateTime.MinValue,
                    EndTime = endsAt ?? DateTime.MaxValue,
                }
            );

            return eventResults.MapEvents();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching data events");
            throw;
        }
    }

    private string GenerateQuery(DateTime? startsAt, DateTime? endsAt)
    {
        var queryBuilder = new StringBuilder(
            @"
            SELECT 
                re.event_id AS Id,
                re.title AS Title,
		        re.start_date AS StartDate,
                re.end_date AS EndDate,
                re.min_price AS MinPrice,
                re.max_price AS MaxPrice
            FROM resumed_events re
            "
        );

        bool hasWhereClause = false;

        if (startsAt.HasValue)
        {
            queryBuilder.Append(hasWhereClause ? " AND" : " WHERE");
            queryBuilder.Append(" re.start_date >= @StartTime");
            hasWhereClause = true;
        }

        if (endsAt.HasValue)
        {
            queryBuilder.Append(hasWhereClause ? " AND" : " WHERE");
            queryBuilder.Append(" re.end_date <= @EndTime");
        }

        return queryBuilder.ToString();
    }
}
