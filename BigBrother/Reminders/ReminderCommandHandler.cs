using Discord.Interactions;
using Eris.Handlers.CommandHandlers;

namespace BigBrother.Commands.Reminder;

[Group("reminder", "truc")]
internal class ReminderCommandHandler : CommandHandler
{
    [SlashCommand("add", "create a new reminder")]
    public async Task Add()
    {
        await RespondAsync("Not yet implemented");
    }

    [SlashCommand("list", "see all of your reminders")]
    public async Task List()
    {
        await RespondAsync("Not yet");
    }

    [SlashCommand("remove", "remove one of your reminder")]
    public async Task Remove()
    {
        await RespondAsync("How many times do I have to tell you ? This is not yet implemented");
    }
}
