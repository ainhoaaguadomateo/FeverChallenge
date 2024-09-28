using System.Runtime.CompilerServices;
using Fever.Domain.Features.Events.Entities;
using Fever.Presentation.Features.Events.DTOs;

namespace Fever.Presentation.Features.Events.Extensions;

public static class BaseEventDTOExtension
{
    public static bool IsValid(this BaseEventDTO baseEvent)
    {
        if (baseEvent.Events.Any())
        {
            foreach (var eventItem in baseEvent.Events)
            {
                var duplicados = eventItem
                    .Zones.GroupBy(z => z.ZoneId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicados.Any())
                {
                    return false;
                }
            }
        }

        return true;
    }
}
