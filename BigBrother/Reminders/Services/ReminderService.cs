using BigBrother.Reminders.Models;
using BigBrother.Reminders.Repositories;

namespace BigBrother.Reminders.Services;

public class ReminderService
{
    private readonly ReminderRepository _reminderRepository;
    private Task _reminderTask;

    public event EventHandler<Reminder>? Machin;

    public ReminderService(ReminderRepository reminderRepository)
    {
        _reminderRepository = reminderRepository;
        _reminderTask = Truc(CancellationToken.None);
    }

    public void AddReminder(Reminder reminder)
    {
        _reminderRepository.Create(reminder);
    }

    public IEnumerable<Reminder> GetReminders(ulong userId)
    {
        return _reminderRepository.GetByUserId(userId);
    }

    public void DeleteReminder()
    {
        throw new NotImplementedException();
    }

    private async Task Truc(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(10000, cancellationToken);
            Reminder? reminder = _reminderRepository.GetNextReminder();
            Console.WriteLine(reminder?.ToString() ?? "Null");
            if (reminder is not null)
            {
                ReminderRepository._reminders.Remove(reminder);
                Machin?.Invoke(this, reminder);
            }
        }
    }
}
