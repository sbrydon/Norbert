using ChatSharp;
using Norbert.Modules.Common;

namespace Norbert.Cli
{
    public class ChatClient : IChatClient
    {
        private readonly IrcClient _client;

        public ChatClient(IrcClient client)
        {
            _client = client;
        }

        public void SendMessage(string message, params string[] destinations)
        {
            _client.SendMessage(message, destinations);
        }
    }
}