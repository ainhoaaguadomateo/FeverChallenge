using Fever.Domain.Features.Events.Entities;
using Fever.Domain.Features.GetEvents.Models;

namespace Fever.Application.Interfaces;

public interface IEventsRepository
{
    Task<GetEventResponseDto> GetEventsAsync(
        DateTime? startsAt,
        DateTime? endsAt,
        CancellationToken cancellationToken = default
    );

    Task SaveUpdatedEvents(List<BaseEvent> baseEvents, CancellationToken cancellationToken);
}
