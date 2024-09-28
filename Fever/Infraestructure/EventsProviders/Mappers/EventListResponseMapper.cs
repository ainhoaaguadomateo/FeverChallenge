using Fever.Domain.Features.GetEvents.Models;

namespace Fever.Infraestructure.EventsProviders.Mappers;

public static class EventListResponseMapper
{
    public static GetEventResponseDto MapEvents(this IEnumerable<GetEventListDTO> eventListDto)
    {
        var eventResultList = eventListDto
            .Select(e =>
            {
                var id = e.Id;

                return new EventResultDTO
                {
                    Id = e.Id,
                    Title = e.Title,

                    StartDate = DateOnly.FromDateTime(e.StartDate),
                    StartTime = TimeOnly.FromDateTime(e.StartDate),

                    EndDate = DateOnly.FromDateTime(e.EndDate),
                    EndTime = TimeOnly.FromDateTime(e.EndDate),

                    MinPrice = e.MinPrice,
                    MaxPrice = e.MaxPrice,
                };
            })
            .ToList();
        return new GetEventResponseDto { Events = eventResultList };
    }
}
