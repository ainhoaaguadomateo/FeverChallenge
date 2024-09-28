namespace Fever.Domain.Features.GetEvents.Models;

public sealed record GetEventListDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
}
