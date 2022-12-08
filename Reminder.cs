﻿using Discord;
using System.Globalization;
using System.Text.RegularExpressions;

internal class Reminder
{
    private readonly static Regex parsingRegex = new Regex("([0-9]{2}/[0-9]{2}/[0-9]{4} [0-9]{2}:[0-9]{2}:[0-9]{2})\n([0-9]+)\n([0-9]+)\n(.+)");
    private readonly static char[] allowedChars = Enumerable.Range('a', 26).Select(c => (char)c).ToArray();

    public string ReminderId;
    public DateTime DateTime;
    public ulong UserId;
    public ulong ChannelId;
    public string Text;

    public static Reminder? Load(string folderPath, string reminderId)
    {
        string text;
        using (StreamReader streamReader = new StreamReader(Path.Combine(folderPath, reminderId)))
            text = streamReader.ReadToEnd();

        if (!parsingRegex.IsMatch(text))
            return null;

        // TODO TryParse may avoid some errors
        GroupCollection data = parsingRegex.Match(text).Groups;
        DateTime dateTime = DateTime.Parse(data[1].Value);
        ulong userId = ulong.Parse(data[2].Value);
        ulong channelId = ulong.Parse(data[3].Value);
        return new Reminder(dateTime, userId, channelId, data[4].Value, reminderId);
    }

    public Reminder(DateTime dateTime, ulong userId, ulong channelId, string text, string reminderId = "")
    {
        ReminderId = reminderId;
        DateTime = dateTime;
        UserId = userId;
        ChannelId = channelId;
        Text = text;
    }

    public override string ToString()
    {
        // TODO some comments would be nice
        return $"{DateTime.ToString("G", CultureInfo.GetCultureInfo("fr-FR"))}\n{UserId}\n{ChannelId}\n{Text}";
    }

    private void SetRandomId(string folderPath, int length = 12)
    {
        Random random = new Random();
        do {
            ReminderId = "";
            for (int i = 0; i < length; i++)
                ReminderId += allowedChars[random.Next(0, allowedChars.Length)];
        } while (File.Exists(Path.Combine(folderPath, ReminderId)));
    }

    public void Save(string folderPath)
    {
        if (ReminderId == "")
            SetRandomId(folderPath);

        string fileName = Path.Combine(folderPath, ReminderId);
        if (File.Exists(fileName))
            throw new Exception("File already exists");
        using (StreamWriter streamWriter = new StreamWriter(fileName))
            streamWriter.WriteLine(ToString());
    }
}

internal partial class BigBrother
{
    private const string REMINDERFOLDER = "Reminders";
    //private const string REMINDERFILE = "Reminders.csv";

    private List<Reminder> reminders;

    private void InitRemindMe()
    {
        LoadReminders();
        commands.Add(new Command("remindMe", " (?:([0-9]+)d)? ?(?:([0-9]+)h)? ?(?:([0-9]+)m)? (.+)", " <duration> <text>` -> Wait `duration` before sending you back `text`", RemindMe));
        Remind();
    }

    private void LoadReminders()
    {
        reminders = new List<Reminder>();
        foreach (string path in Directory.GetFiles(GetPath(REMINDERFOLDER)))
        {
            Reminder? newReminder = Reminder.Load(GetPath(REMINDERFOLDER), Path.GetFileName(path));
            if (newReminder != null)
                reminders.Add(newReminder);
        }
    }

    private async Task RemindMe(IMessage message, GroupCollection args)
    {
        DateTime reminderDate = DateTime.Now;
        if (args[1].Value != "")
        {
            double days;
            if (!double.TryParse(args[1].Value, out days) || days > 365000)
            {
                await SendMessage(message.Channel, "No need, you'll be dead by then");
                return;
            }
            reminderDate = reminderDate.AddDays(days);
        }
        if (args[2].Value != "")
        {
            double hours;
            if (!double.TryParse(args[3].Value, out hours))
            {
                await SendMessage(message.Channel, "Invalid duration");
                return;
            }
            reminderDate = reminderDate.AddHours(hours);
        }
        if (args[3].Value != "")
        {
            double minutes;
            if (!double.TryParse(args[3].Value, out minutes))
            {
                await SendMessage(message.Channel, "Invalid duration");
                return;
            }
            reminderDate = reminderDate.AddMinutes(minutes);
        }

        Reminder newReminder = new Reminder(reminderDate, message.Author.Id, message.Channel.Id, args[4].Value);
        reminders.Add(newReminder);
        newReminder.Save(GetPath(REMINDERFOLDER));
        await SendMessage(message.Channel, $"{{{message.Author.Id}}}: Reminder added for the {reminderDate.ToString("G", CultureInfo.GetCultureInfo("fr-FR"))}");
    }

    private async void Remind()
    {
        // Make this method async
        await Task.Yield();

        // Wait for the bot to be ready
        while (!IsReady) { }

        while (IsRunning)
        {
            // TODO might crash if disconnected

            // List of all reminders that have been reminded and must be removed
            List<Reminder> reminded = new List<Reminder>();

            foreach (Reminder reminder in reminders)
            {
                if (reminder.DateTime <= DateTime.Now)
                {
                    await SendMessage(reminder.ChannelId, $"{{{reminder.UserId}}}, I have to remind you:\n> {reminder.Text}");
                    reminded.Add(reminder);
                }
            }

            foreach (Reminder reminder in reminded)
            {
                reminders.Remove(reminder);
                File.Delete(GetPath(REMINDERFOLDER, reminder.ReminderId));
            }

            await Task.Delay(60000);
        }
    }
}