using Discord;
using Discord.WebSocket;
using Eris.Handlers.MessageHandlers;
using Eris.Logging;
using static BigBrother.Conversation.OllamaClient;
using static BigBrother.Conversation.OllamaClient.OllamaRequest;

namespace BigBrother.Conversation;

public class ConversationMessageHandler : IMessageHandler
{
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
            new Message(previousMessage.Author.Id == _client.CurrentUser.Id ? Message.Role.Assistant : Message.Role.User,
                $"User {(message.Author as IGuildUser)!.DisplayName}: {message}"))
        ));
        if (response is null)
        {
            await _logger.Log(LogSeverity.Warning, nameof(ConversationMessageHandler), "No response from LLM");
            return true;
        }

        await message.Channel.SendMessageAsync(response.Take(2000).ToString());
        return true;
    }
}
