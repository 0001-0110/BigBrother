using BigBrother.Reminders.Models;
using Npgsql;

namespace BigBrother.Reminders.Repositories;

public class ReminderRepository : SqlRepository
{
    public ICollection<Reminder> _reminders;

    // TODO Replace by value from configuration
    public ReminderRepository() : base("Host=bigbrother-postgres;Port=5432;Username=postgres;Password=password;Database=postgres")
    {
        _reminders = [];
    }

    private async Task<Reminder?> MapReminder(NpgsqlDataReader reader)
    {
        if (!await reader.ReadAsync())
            return null;

        return new Reminder(
            id: reader.GetGuid(0),
            userId: Convert.ToUInt64(reader.GetValue(1)),
            channelId: Convert.ToUInt64(reader.GetValue(2)),
            dueDate: reader.GetDateTime(3),
            message: reader.GetString(4)
        );
    }

    private async Task<IEnumerable<Reminder>> MapReminders(NpgsqlDataReader reader)
    {
        List<Reminder> reminders = [];
        while (await reader.ReadAsync())
        {
            reminders.Add(new Reminder(
                id: reader.GetGuid(0),
                userId: Convert.ToUInt64(reader.GetValue(1)),
                channelId: Convert.ToUInt64(reader.GetValue(2)),
                dueDate: reader.GetDateTime(3),
                message: reader.GetString(4)
            ));
        }
        return reminders;
    }

    public Task<Reminder> Create(Reminder reminder)
    {
        NpgsqlCommand createCommand(NpgsqlConnection connection)
        {
            // TODO Change to stored procedures
            NpgsqlCommand command = new NpgsqlCommand(
                @"INSERT INTO reminders (user_id, channel_id, due_date, message)
                VALUES (@userId, @channelId, @dueDate, @message)
                RETURNING id, user_id, channel_id, due_date, message",
                connection);

            command.Parameters.AddWithValue("userId", NpgsqlTypes.NpgsqlDbType.Numeric, Convert.ToDecimal(reminder.UserId));
            command.Parameters.AddWithValue("channelId", NpgsqlTypes.NpgsqlDbType.Numeric, Convert.ToDecimal(reminder.ChannelId));
            command.Parameters.AddWithValue("dueDate", reminder.DueDate);
            command.Parameters.AddWithValue("message", reminder.Message);

            return command;
        }

        return Execute(createCommand, MapReminder);
    }

    public Task<IEnumerable<Reminder>> GetByUserId(ulong userId)
    {
        NpgsqlCommand createCommand(NpgsqlConnection connection)
        {
            // TODO Change to stored procedures
            NpgsqlCommand command = new NpgsqlCommand(
                @"SELECT id, user_id, channel_id, due_date, message
                FROM reminders
                WHERE user_id = @userId",
                connection);

            command.Parameters.AddWithValue("userId", NpgsqlTypes.NpgsqlDbType.Numeric, Convert.ToDecimal(userId));

            return command;
        }

        return Execute(createCommand, MapReminders);
    }

    public Task<Reminder?> GetNextDueReminder()
    {
        NpgsqlCommand createCommand(NpgsqlConnection connection)
        {
            // TODO Change to stored procedures
            return new NpgsqlCommand(
                @"SELECT id, user_id, channel_id, due_date, message
                FROM reminders
                ORDER BY due_date
                LIMIT 1",
                connection);
        }

        return Execute(createCommand, MapReminder);
    }

    public async Task<bool> Delete(Guid id)
    {
        NpgsqlCommand createCommand(NpgsqlConnection connection)
        {
            NpgsqlCommand command = new NpgsqlCommand(@"DELETE FROM reminders WHERE id = @id", connection);
            command.Parameters.AddWithValue("id", id);
            return command;
        }

        return await ExecuteNonQuery(createCommand) != 0;
    }
}
