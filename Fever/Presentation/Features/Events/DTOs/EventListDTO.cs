using System.Xml.Serialization;

namespace Fever.Presentation.Features.Events.DTOs;

[XmlRoot(ElementName = "eventList", Namespace = "", IsNullable = false)]
public class EventListDTO
{
    [XmlElement(ElementName = "output")]
    public OutputDTO Output { get; set; } = default!;
}

[XmlRoot(ElementName = "output")]
public class OutputDTO
{
    [XmlElement(ElementName = "base_event")]
    public List<BaseEventDTO> BaseEvents { get; set; } = [];
}

[XmlRoot(ElementName = "base_event")]
public class BaseEventDTO
{
    [XmlAttribute(AttributeName = "base_event_id")]
    public int BaseEventId { get; set; }

    [XmlAttribute(AttributeName = "sell_mode")]
    public string SellMode { get; set; } = default!;

    [XmlAttribute(AttributeName = "title")]
    public string Title { get; set; } = default!;

    [XmlElement(ElementName = "event")]
    public List<EventDTO> Events { get; set; } = [];
}

[XmlRoot(ElementName = "event")]
public class EventDTO
{
    [XmlAttribute(AttributeName = "event_start_date")]
    public string EventStartDate { get; set; } = default!;

    [XmlAttribute(AttributeName = "event_end_date")]
    public string EventEndDate { get; set; } = default!;

    [XmlAttribute(AttributeName = "event_id")]
    public int EventId { get; set; }

    [XmlAttribute(AttributeName = "sell_from")]
    public string SellFrom { get; set; } = default!;

    [XmlAttribute(AttributeName = "sell_to")]
    public string SellTo { get; set; } = default!;

    [XmlAttribute(AttributeName = "sold_out")]
    public bool SoldOut { get; set; }

    [XmlElement(ElementName = "zone")]
    public List<ZoneDTO> Zones { get; set; } = [];
}

[XmlRoot(ElementName = "zone")]
public class ZoneDTO
{
    [XmlAttribute(AttributeName = "zone_id")]
    public int ZoneId { get; set; }

    [XmlAttribute(AttributeName = "capacity")]
    public int Capacity { get; set; }

    [XmlAttribute(AttributeName = "price")]
    public decimal Price { get; set; }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; } = default!;

    [XmlAttribute(AttributeName = "numbered")]
    public bool Numbered { get; set; }
}
