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
        // while (!cancellationToken.IsCancellationRequested)
        // {
        //     Console.WriteLine("Waiting");
        //     Reminder reminder = await _reminderService.GetNextReminder(cancellationToken);
        //     Console.WriteLine($"Reminding: {reminder}");
        //     await UserExtensions.SendMessageAsync(await _client.GetUserAsync(reminder.UserId), reminder.ToString());
        // }

        // await foreach (Reminder reminder in _reminderService.GetDueReminders(CancellationToken.None))
        //     await UserExtensions.SendMessageAsync(await _client.GetUserAsync(reminder.UserId), reminder.ToString());
    }
}
