using BigBrother.Reminders.Models;
using BigBrother.Reminders.Repositories;

namespace BigBrother.Reminders.Services;

public class ReminderService
{
    private readonly ReminderRepository _reminderRepository;

    public ReminderService(ReminderRepository reminderRepository)
    {
        _reminderRepository = reminderRepository;
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

    public async Task<Reminder> GetNextReminder(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(10000, cancellationToken);
            Reminder? reminder = ReminderRepository._reminders.FirstOrDefault(reminder => reminder.DueDate < DateTime.Now);
            if (reminder is not null)
            {
                ReminderRepository._reminders.Remove(reminder);
                return reminder;
            }
        }
        return null;
    }
}
