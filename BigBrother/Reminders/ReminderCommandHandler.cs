using BigBrother.Reminders.Models;
using BigBrother.Reminders.Services;
using Discord.Interactions;
using Discord.WebSocket;
using Eris.Handlers.CommandHandlers;
using Eris.Logging;

namespace BigBrother.Reminders;

[Group("reminder", "truc")]
internal class ReminderCommandHandler : CommandHandler
{
    // TODO Move this to the utilities
    public static bool TryParseReminderTime(string input, out DateTime dateTime)
    {
        if (TimeSpan.TryParse(input, out TimeSpan ts))
        {
            dateTime = DateTime.Now.Add(ts);
            return true;
        }

        if (DateTime.TryParse(input, out DateTime dt))
        {
            dateTime = dt;
            return true;
        }

        // if (input.StartsWith("in "))
        //     {
        //         var span = input[3..];
        //         if (TimeSpan.TryParse(span, out var rel))
        //             return DateTime.Now.Add(rel);
        //     }

        // Optionally, use Humanizer or other NLP parser here

        dateTime = DateTime.UnixEpoch;
        return false;
    }

    private readonly ILogger _logger;
    private readonly ReminderService _reminderService;

    public ReminderCommandHandler(DiscordSocketClient client, ILogger logger, ReminderService reminderService)
    {
        _logger = logger;
        _reminderService = reminderService;
    }

    [SlashCommand("add", "create a new reminder")]
    public async Task Add(string when, string message)
    {
        if (!TryParseReminderTime(when, out DateTime dueDate))
        {
            await RespondAsync("Invalid date or timespan");
            return;
        }

        string result = await _reminderService.AddReminder(new Reminder(Context.User.Id, Context.Channel.Id, dueDate, message));
        await RespondAsync(result);
    }

    [SlashCommand("list", "see all of your reminders")]
    public async Task List()
    {
        string response = string.Join(Environment.NewLine, (await _reminderService.GetReminders(Context.User.Id)).Select(reminder => reminder.ToString()));
        await RespondAsync($"Reminders:\n{response}");
    }

    // [SlashCommand("remove", "remove one of your reminder")]
    public async Task Remove(string id)
    {
        await RespondAsync("How many times do I have to tell you ? This is not yet implemented");
    }
}
