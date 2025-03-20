using Discord;
using Eris;

namespace BigBrother;

public static class Program
{
    public static async Task Main()
    {
        ErisClient eris = new ErisClient();

        //eris.AddCommandHandler<HelloCommandHandler>();

        eris.Client.Ready += async () =>
        {
            await eris.Client.SetStatusAsync(UserStatus.Online);
            await eris.Client.SetGameAsync("you (5.0)", type: ActivityType.Watching);
        };
        await eris.Run();
    }
}
