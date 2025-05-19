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
        Console.WriteLine(userId);
        return _reminderRepository.GetByUserId(userId);
    }

    public void DeleteReminder()
    {
        throw new NotImplementedException();
    }
}
