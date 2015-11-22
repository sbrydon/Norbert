using Norbert.Modules.Common;

namespace Norbert.Modules.ChatLog
{
    public class ChatLog : INorbertModule
    {
        private IChatClient _client;

        public void Loaded(IChatClient client)
        {
            _client = client;
            _client.MessageReceived += OnMessageReceived;
        }

        public void Unloaded()
        {
            
        }

        private void OnMessageReceived(object sender, MessageEventArgs eventArgs)
        {
            _client.SendMessage($"{eventArgs.Nick} said: {eventArgs.Message}", eventArgs.Source);
        }
    }
}
