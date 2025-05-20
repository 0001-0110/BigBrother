using BigBrother.Reminders.Models;

namespace BigBrother.Reminders.Repositories;

public class ReminderRepository
{
    public static ICollection<Reminder> _reminders = [];

    public ReminderRepository()
    {
        // _reminders = [];
    }

    public void Create(Reminder reminder)
    {
        _reminders.Add(reminder);
    }

    public IEnumerable<Reminder> GetByUserId(ulong userId)
    {
        return _reminders.Where(reminder => reminder.UserId == userId);
    }

    public Reminder? GetNextReminder()
    {
        return _reminders.FirstOrDefault(reminder => reminder.DueDate < DateTime.Now);
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }
}
