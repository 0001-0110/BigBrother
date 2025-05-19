using BigBrother.Reminders.Models;

namespace BigBrother.Reminders.Repositories;

public class ReminderRepository
{
    private static ICollection<Reminder> _reminders = [];

    public ReminderRepository()
    {
        // _reminders = [];
    }

    public void Create(Reminder reminder)
    {
        _reminders.Add(reminder);

        foreach (var truc in _reminders)
            Console.WriteLine(truc.ToString());
    }

    public IEnumerable<Reminder> GetByUserId(ulong userId)
    {
        Console.WriteLine(_reminders.Count);
        foreach (var reminder in _reminders)
            Console.WriteLine(reminder.ToString());

        return _reminders.Where(reminder => reminder.UserId == userId);
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }
}
