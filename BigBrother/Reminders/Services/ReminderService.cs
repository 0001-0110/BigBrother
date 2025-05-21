using BigBrother.Reminders.Models;
using BigBrother.Reminders.Repositories;
using Discord;
using Discord.WebSocket;

namespace BigBrother.Reminders.Services;

public class ReminderService
{
    private readonly DiscordSocketClient _client;
    private readonly ReminderRepository _reminderRepository;
    private CancellationTokenSource _cancellationTokenSource;
    private Task _reminderTask;

    public ReminderService(DiscordSocketClient client, ReminderRepository reminderRepository)
    {
        _client = client;
        _reminderRepository = reminderRepository;
        _cancellationTokenSource = new CancellationTokenSource();
        _reminderTask = Task.CompletedTask;
    }

    public async Task<string> AddReminder(Reminder reminder)
    {
        // TODO This is cumbersome, move or remove
        if (reminder.DueDate < DateTime.Now)
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

            return errors[new Random().Next(errors.Length)];
        }

        _reminderRepository.Create(reminder);

        _cancellationTokenSource.Cancel();
        await _reminderTask;
        _cancellationTokenSource = new CancellationTokenSource();
        _reminderTask = WaitForNextReminder(_cancellationTokenSource.Token);

        return $"Added reminder for {TimestampTag.FromDateTime(reminder.DueDate)}";
    }

    public IEnumerable<Reminder> GetReminders(ulong userId)
    {
        return _reminderRepository.GetByUserId(userId);
    }

    public void DeleteReminder()
    {
        throw new NotImplementedException();
    }

    private async Task WaitForNextReminder(CancellationToken cancellationToken)
    {
        Reminder? reminder = _reminderRepository.GetNextDueReminder();
        if (reminder is null)
            return;

        TimeSpan delay = reminder.DueDate - DateTime.Now;
        try
        {
            // Only wait if the timespan is positive, since a negative timespan would mean to wait forever
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        // TODO Layer separation violation, move to somewhere else
        // TODO Rewrite this mess
        await (await _client.GetChannelAsync(reminder.ChannelId) as IMessageChannel)!.SendMessageAsync(reminder.ToString());

        _reminderRepository.Delete(reminder);
        _reminderTask = WaitForNextReminder(cancellationToken);
    }
}
