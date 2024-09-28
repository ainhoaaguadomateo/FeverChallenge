using Fever.Domain.Features.Events.Entities;

namespace Fever.Application.Interfaces;

public interface IExternalEventService
{
    Task<List<BaseEvent>> FetchEventsAsync(CancellationToken cancellationToken);
}
