using Eris;

namespace BigBrother;

public static class Program
{
    public static async Task Main()
    {
        ErisClient client = new ErisClient();

        await client.Run();
    }
}
