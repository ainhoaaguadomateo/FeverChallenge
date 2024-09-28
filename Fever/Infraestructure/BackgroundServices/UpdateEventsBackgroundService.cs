using Fever.Application.Features.Events.Commands.UpdateEvents;
using MediatR;

namespace Fever.Infraestructure.BackgroundServices;

public class UpdateEventsBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<UpdateEventsBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(5));

        UpdateEventsCommand command = new();

        while (
            !cancellationToken.IsCancellationRequested
            && await timer.WaitForNextTickAsync(cancellationToken)
        )
        {
            using var scope = serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            try
            {
                // Gets the events from the external provider and save it to database.
                await mediator.Send(command, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }
    }
}
