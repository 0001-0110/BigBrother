using BigBrother.Reminders.Models;
using BigBrother.Reminders.Services;
using Discord;
using Discord.WebSocket;
using Eris.Handlers.BackgroundTasks;

namespace BigBrother.Reminders;

internal class ReminderBackgroundTaskHandler : IBackgroundTaskHandler
{
    private readonly DiscordSocketClient _client;
    private readonly ReminderService _reminderService;

    public ReminderBackgroundTaskHandler(DiscordSocketClient client, ReminderService reminderService)
    {
        _client = client;
        _reminderService = reminderService;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Reminder reminder = await _reminderService.GetNextReminder(cancellationToken);
            await UserExtensions.SendMessageAsync(await _client.GetUserAsync(reminder.UserId), reminder.ToString());
        }
    }
}
