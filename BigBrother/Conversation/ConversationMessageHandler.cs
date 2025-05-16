using Discord;
using Discord.WebSocket;
using Eris.Handlers.Messages;
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
        string? response = await _ollamaClient.Generate(new OllamaRequest([
            new Message(Message.Role.User, message.Content),
        ]));
        if (response is null)
        {
            await _logger.Log(LogSeverity.Warning, nameof(ConversationMessageHandler), "No response from LLM");
            return true;
        }

        await message.Channel.SendMessageAsync(response);
        return true;
    }
}
