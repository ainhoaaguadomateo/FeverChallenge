using System.Text.Json.Serialization;
using Fever.Domain.Features.GetEvents.Models;
using MediatR;

namespace Fever.Application.Features.Events.Queries.GetEvents;

public sealed record GetEventsQuery(
    [property: JsonPropertyName("starts_at")] DateTime? StartsAt = null,
    [property: JsonPropertyName("ends_at")] DateTime? EndsAt = null
) : IRequest<GetEventResponseDto>;
