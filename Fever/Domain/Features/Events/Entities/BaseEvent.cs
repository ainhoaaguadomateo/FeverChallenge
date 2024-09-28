using System.Xml.Serialization;

namespace Fever.Domain.Features.Events.Entities;

public class BaseEvent
{
    public int BaseEventId { get; set; }

    public string SellMode { get; set; } = default!;

    public string Title { get; set; } = default!;

    public List<Event> Events { get; set; } = [];
}
