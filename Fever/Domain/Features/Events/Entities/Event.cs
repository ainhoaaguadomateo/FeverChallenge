using Fever.Domain.Features.GetEvents.Entities;

namespace Fever.Domain.Features.Events.Entities;

public class Event
{
    public Guid Id { get; set; }
    public int EventId { get; set; }
    public int BaseEventId { get; set; }

    public DateTime EventStartDate { get; set; }

    public DateTime EventEndDate { get; set; }

    public BaseEvent BaseEvent { get; set; } = default!;

    public DateTime SellFrom { get; set; }

    public DateTime SellTo { get; set; }

    public bool SoldOut { get; set; }

    public List<Zone> Zones { get; set; } = [];

    public ResumedEvent? ResumedEvent { get; set; } = default!;
}
