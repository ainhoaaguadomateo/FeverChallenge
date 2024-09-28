using Fever.Domain.Features.Events.Entities;

namespace Fever.Domain.Features.GetEvents.Entities;

public class ResumedEvent
{
    public Guid EventId { get; set; }
    public string Title { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public Event Event { get; set; } = default!;
}
