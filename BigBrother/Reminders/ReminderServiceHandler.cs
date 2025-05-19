using Eris.Handlers.BackgroundTasks;

namespace BigBrother.Reminders;

internal class ReminderBackgroundTaskHandler : IBackgroundTaskHandler
{
    public Task Run(CancellationToken cancellationToken)
    {
        return Task.Delay(-1, cancellationToken);
    }
}
