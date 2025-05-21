using BigBrother.Reminders.Models;

namespace BigBrother.Reminders.Repositories;

public class ReminderRepository
{
    public ICollection<Reminder> _reminders;

    public ReminderRepository()
    {
        _reminders = [];
    }

    public void Create(Reminder reminder)
    {
        _reminders.Add(reminder);
    }

    public IEnumerable<Reminder> GetByUserId(ulong userId)
    {
        return _reminders.Where(reminder => reminder.UserId == userId);
    }

    public Reminder? GetNextDueReminder()
    {
        return _reminders.MinBy(reminder => reminder.DueDate);
    }

    public void Delete(Reminder reminder)
    {
        _reminders.Remove(reminder);
    }
}
