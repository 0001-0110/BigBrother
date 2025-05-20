namespace BigBrother.Reminders.Models;

public class Reminder(ulong userId, ulong channelId, DateTime dueDate, string message)
{
    public ulong UserId { get; set; } = userId;

    public ulong ChannelId { get; set; } = channelId;

    public DateTime DueDate { get; set; } = dueDate;

    public string Message { get; set; } = message;

    public override string ToString()
    {
        return $"{DueDate}: {Message}";
    }
}
