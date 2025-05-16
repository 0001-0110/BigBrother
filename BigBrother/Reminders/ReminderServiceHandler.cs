using Eris.Handlers.Services;

namespace BigBrother.Reminders;

internal class ReminderServiceHandler : IServiceHandler
{
    public Task Run(CancellationToken cancellationToken)
    {
        return Task.Delay(-1, cancellationToken);
    }
}
