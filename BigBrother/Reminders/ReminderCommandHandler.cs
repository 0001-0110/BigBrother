using BigBrother.Reminders.Models;
using BigBrother.Reminders.Services;
using Discord;
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
        _reminderService.Machin += async (sender, reminder) => await UserExtensions.SendMessageAsync(await client.GetUserAsync(reminder.UserId), reminder.ToString());
    }

    [SlashCommand("add", "create a new reminder")]
    public async Task Add(string when, string message)
    {
        if (!TryParseReminderTime(when, out DateTime dueDate))
        {
            await RespondAsync("Invalid date or timespan");
            return;
        }

        // TODO Move this in the service
        if (dueDate < DateTime.Now)
        {
            string[] errors = [
                "Setting reminders in the past — ambitious, if misguided.",
                "Ah, nostalgia. Try a future time instead.",
                "I admire your commitment to irrelevance.",
                "Past reminders? The future called, it's confused.",
                "Time travel isn't supported. Yet.",
                "That moment already happened. Try again.",
                "Living in the past, are we?",
                "Your sense of timing is... unique.",
                "Remind me never to ask you for the time.",
                "If only you could change the past. Spoiler: You can't.",
                "Past reminders? Bold strategy.",
                "You really like reruns, huh?",
                "Future's where the magic happens. Try there.",
                "I'll remember that you're hopeless with time.",
                "Error 404: Future timestamp not found.",
                "Your calendar is as reliable as your jokes.",
                "That's not how time works, friend.",
                "Try to think forward, it's a wild concept.",
                "Past reminders: impressively pointless.",
                "Set it for later — you might surprise yourself.",
            ];

            await RespondAsync(errors[new Random().Next(errors.Length)]);
        }

        _reminderService.AddReminder(new Reminder(Context.User.Id, dueDate, message));
        await RespondAsync($"Added reminder for {TimestampTag.FromDateTime(dueDate)}");
    }

    [SlashCommand("list", "see all of your reminders")]
    public async Task List()
    {
        string response = string.Join(Environment.NewLine, _reminderService.GetReminders(Context.User.Id).Select(reminder => reminder.ToString()));
        await RespondAsync($"Reminders:\n{response}");
    }

    [SlashCommand("remove", "remove one of your reminder")]
    public async Task Remove(string id)
    {
        await RespondAsync("How many times do I have to tell you ? This is not yet implemented");
    }
}
