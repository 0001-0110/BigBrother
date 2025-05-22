namespace BigBrother.Reminders.Models;

public class Reminder
{
    public Guid Id { get; set; }

    public ulong UserId { get; set; }

    public ulong ChannelId { get; set; }

    public DateTime DueDate { get; set; }

    public string Message { get; set; }

    public Reminder(ulong userId, ulong channelId, DateTime dueDate, string message)
    {
        UserId = userId;
        ChannelId = channelId;
        DueDate = dueDate;
        Message = message;
    }

    public Reminder(Guid id, ulong userId, ulong channelId, DateTime dueDate, string message) : this(userId, channelId, dueDate, message)
    {
        Id = id;
    }

    public override string ToString()
    {
        return $"{DueDate}: {Message}";
    }
}
