using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Eris.Handlers.MessageHandlers;
using Eris.Logging;
using static BigBrother.Conversation.OllamaClient;
using static BigBrother.Conversation.OllamaClient.OllamaRequest;
using static BigBrother.Conversation.OllamaClient.OllamaRequest.Message;

namespace BigBrother.Conversation;

public class ConversationMessageHandler : IMessageHandler
{
    public static T AwaitSync<T>(Task<T> task)
    {
        task.Wait();
        return task.Result;
    }

    /// <summary>
    /// Replaces all mentions by the current display name of the user
    /// </summary>
    /// <param name="message">The message received</param>
    /// <returns>The new message with all mentions replaced</returns>
    public static string GetPreProcessedContent(IMessage message)
    {
        // Search for all mentions
        return new Regex("<@[0-9]+>").Replace(message.Content,
            // Replaces it by the display name of the user with the mathcing id
            // [2..^1] captures the id inside (by removing the '@<' and the '>')
            match => (AwaitSync(message.Channel.GetUserAsync(ulong.Parse(match.Value[2..^1]))) as IGuildUser)!.DisplayName);
    }

    private const string _errorMessage = "Oh no! The squirrels have taken over the server room again! 🐿️🚨\n**Error 503**: Server Room Occupied by Squirrels\n```Description: We apologize for the interruption, but it seems our servers are currently experiencing a rodent-induced outage. Our team is frantically chasing them out with acorns and motivational speeches. Please bear with us as we restore order and get back to serving you shortly! If problem persists, please contact our tech support and mention you've encountered the \"Squirrelpocalypse Error.\"```";
    private const string _prompt = "You are a discord bot named Big Brother. Your task is to be helpful when someone asks you a question, and to be funny otherwise, using a dry sens of humor. Keep the messages short, and always start by 'User Big Brother:'";

    private readonly DiscordSocketClient _client;
    private readonly ILogger _logger;
    private readonly OllamaClient _ollamaClient;

    public ConversationMessageHandler(DiscordSocketClient client, ILogger logger, OllamaClient ollamaClient)
    {
        _client = client;
        _logger = logger;
        _ollamaClient = ollamaClient;
    }

    public async Task<bool> Handle(SocketMessage message)
    {
        if (!message.MentionedUsers.Select(user => user.Id).Contains(_client.CurrentUser.Id))
            return false;

        using IDisposable typing = message.Channel.EnterTypingState();
        string? response = await _ollamaClient.Generate(new OllamaRequest(
            (await message.Channel.GetMessagesAsync().FlattenAsync()).Select(previousMessage =>
            new Message(previousMessage.Author.Id == _client.CurrentUser.Id ? Role.Assistant : Role.User,
                $"User {(message.Author as IGuildUser)!.DisplayName}: {GetPreProcessedContent(message)}"))
                // Add the prompt to give the bot its personnality
                .Prepend(new Message(Role.System, _prompt))
        ));
        if (response is null)
            await _logger.Log(LogSeverity.Warning, nameof(ConversationMessageHandler), "No response from LLM");

        await message.Channel.SendMessageAsync(response?[0..Math.Min(response.Length, 2000)] ?? _errorMessage);
        return true;
    }
}
