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
        _reminderTask = WaitForNextReminder(_cancellationTokenSource.Token);
    }

    public async Task<string> AddReminder(Reminder reminder)
    {
        // TODO This is cumbersome, move or remove
        if (reminder.DueDate < DateTime.Now)
            return "Past reminders? Bold strategy.";

        if (reminder.DueDate - DateTime.Now > TimeSpan.FromDays(365))
            return "This is a really long time";

        await _reminderRepository.Create(reminder);

        // TODO Whould it be better to only cancel when needed ? (when the added reminder is sonner than the next reminder)
        _cancellationTokenSource.Cancel();
        await _reminderTask;
        _cancellationTokenSource = new CancellationTokenSource();
        _reminderTask = WaitForNextReminder(_cancellationTokenSource.Token);

        return $"Added reminder for {TimestampTag.FromDateTime(reminder.DueDate)}";
    }

    public Task<IEnumerable<Reminder>> GetReminders(ulong userId)
    {
        return _reminderRepository.GetByUserId(userId);
    }

    public void DeleteReminder()
    {
        throw new NotImplementedException();
    }

    private async Task WaitForNextReminder(CancellationToken cancellationToken)
    {
        Reminder? reminder = await _reminderRepository.GetNextDueReminder();
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

        await _reminderRepository.Delete(reminder.Id);
        _reminderTask = WaitForNextReminder(cancellationToken);
    }
}
