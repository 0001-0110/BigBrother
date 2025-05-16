using System.Text;
using Newtonsoft.Json;

namespace BigBrother.Conversation;

public class OllamaClient
{
    private static readonly string _url = "http://ollama:11434/api/chat";

    public class OllamaRequest
    {
        public class Message
        {
            public enum Role
            {
                System,
                Assistant,
                User,
            }

            [JsonProperty("role")]
            private string _role;

            [JsonProperty("content")]
            private string _content;

            public Message(Role role, string content)
            {
                _role = role.ToString().ToLower();
                _content = content;
            }
        }

        [JsonProperty("model")]
        public string Model { get; } = "llama3";

        [JsonProperty("stream")]
        private bool _stream = false;

        [JsonProperty("messages")]
        private IEnumerable<Message> _messages;

        public OllamaRequest(IEnumerable<Message> messages)
        {
            _messages = messages;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    private class LlamaResponse
    {
        public class ApiMessage
        {
            [JsonProperty("content")]
            public string Content;
        }

        [JsonProperty("message")]
        public ApiMessage Message;
    }

    private async Task Pull(string model)
    {
        string url = "http://ollama:11434/api/pull";
        string json = $"{{\"model\": \"{model}\"}}";
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Response:");
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error:");
            Console.WriteLine(ex.Message);
        }
    }

    public async Task<string?> Generate(OllamaRequest request, bool retry = true)
    {
        string body = request.ToJson();
        StringContent content = new StringContent(body, Encoding.UTF8, "application/json");

        using (HttpClient httpClient = new HttpClient())
        {
            // Send the request
            HttpResponseMessage response = await httpClient.PostAsync(_url, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("LLM request failed");
                Console.WriteLine(response.ToString());
                if (!retry)
                    return null;

                await Pull(request.Model);
                return await Generate(request, false);
            }

            return JsonConvert.DeserializeObject<LlamaResponse>(await response.Content.ReadAsStringAsync())?.Message.Content;
        }
    }
}
