using BigBrother.Commands.Reminder;
using BigBrother.Reminders;
using Discord;
using Eris;
using Eris.Handlers.CommandHandlers.BuiltIn;
using Eris.Handlers.Messages.BuiltIn;
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
            .AddMessageHandler<EchoMessageHandler>()
            .AddCommandHandler<HelloCommandHandler>()
            .AddCommandHandler<VersionCommandHandler>();

        ErisClient eris = builder.Build();
        eris.Client.Ready += async () =>
        {
            await eris.Client.SetStatusAsync(UserStatus.Online);
            await eris.Client.SetGameAsync("you (5.0)", type: ActivityType.Watching);
        };
        await eris.Run();
    }
}
