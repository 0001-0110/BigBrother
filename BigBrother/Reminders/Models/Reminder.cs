namespace BigBrother.Reminders.Models;

public class Reminder(ulong userId, DateTime dueDate, string message)
{
    public ulong UserId { get; set; } = userId;

    public DateTime DueDate { get; set; } = dueDate;

    public string Message { get; set; } = message;

    public override string ToString()
    {
        return $"({UserId}) {DueDate}: {Message}";
    }
}
