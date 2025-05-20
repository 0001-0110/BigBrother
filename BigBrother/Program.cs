using BigBrother.Conversation;
using BigBrother.Reminders;
using BigBrother.Reminders.Repositories;
using BigBrother.Reminders.Services;
using Discord;
using Eris;
using Eris.Handlers.CommandHandlers.BuiltIn;
using Eris.Handlers.MessageHandlers.BuiltIn;
using Eris.Logging;
using Microsoft.Extensions.Configuration;

namespace BigBrother;

public static class Program
{
    public static async Task Main()
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        ErisClientBuilder builder = new ErisClientBuilder()
            .WithConfiguration(config.GetSection("Eris"))
            .AddLogger<ConsoleLogger>()

            .AddMessageHandler<IgnoreBotsMessageHandler>()

            .AddCommandHandler<VersionCommandHandler>()

            .AddService<OllamaClient>()
            .AddMessageHandler<ConversationMessageHandler>()

            .AddService<ReminderRepository>()
            .AddService<ReminderService>()
            .AddCommandHandler<ReminderCommandHandler>();
            // .AddBackgroundTaskHandler<ReminderBackgroundTaskHandler>();

        ErisClient eris = builder.Build();
        eris.Client.Ready += async () =>
        {
            await eris.Client.SetStatusAsync(UserStatus.Online);
            await eris.Client.SetGameAsync("you", type: ActivityType.Watching);
        };
        await eris.Run();
    }
}
