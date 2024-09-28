using System.Xml.Serialization;
using Fever.Application.Interfaces;
using Fever.Domain.Features.Events.Entities;
using Fever.Presentation.Features.Events.DTOs;

namespace Fever.Infraestructure.EventsProviders.UpdateEvents.Services;

public class ExternalEventService : IExternalEventService
{
    private readonly HttpClient _httpClient;
    private readonly IEventsRepository _eventsRepository;
    private readonly IFetchEventsService _fetchEventService;

    public ExternalEventService(
        HttpClient httpClient,
        IEventsRepository eventsRepository,
        IFetchEventsService fetchEventService
    )
    {
        _httpClient = httpClient;
        _eventsRepository = eventsRepository;
        _fetchEventService = fetchEventService;
    }

    public async Task<List<BaseEvent>> FetchEventsAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetStringAsync(
            "https://provider.code-challenge.feverup.com/api/events"
        );

        var serializer = new XmlSerializer(typeof(EventListDTO));
        using (var reader = new StringReader(response))
        {
            var eventList = (EventListDTO)serializer.Deserialize(reader)!;

            var baseEventList = _fetchEventService.GetBaseEventList(eventList);

            return baseEventList;
        }
    }
}
