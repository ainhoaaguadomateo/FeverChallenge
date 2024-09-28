namespace Fever.Domain.Features.Events.Entities;

public class Zone
{
    public Guid Id { get; set; }
    public int ZoneId { get; set; }

    public int Capacity { get; set; }

    public decimal Price { get; set; }

    public string Name { get; set; } = default!;

    public bool Numbered { get; set; }

    public Guid EventId { get; set; }

    public Event Event { get; set; } = default!;
}
