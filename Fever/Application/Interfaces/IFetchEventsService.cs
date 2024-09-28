using Fever.Domain.Features.Events.Entities;
using Fever.Presentation.Features.Events.DTOs;

namespace Fever.Application.Interfaces;

public interface IFetchEventsService
{
    List<BaseEvent> GetBaseEventList(EventListDTO eventListDto);
}
