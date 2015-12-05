using System;
using log4net;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;

namespace Norbert.Modules.ChatLog
{
    public class ChatLogger
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChatLogger));
        private readonly IFileSystem _fileSystem;
        private readonly string _path;

        public ChatLogger(IFileSystem fileSystem, IChatClient chatClient, Config config)
        {
            _fileSystem = fileSystem;
            _path = config.Path;

            chatClient.MessageReceived += (s, e) => AppendToLog(e);
            chatClient.MessageSent += (s, e) => AppendToLog(e);
        }

        private void AppendToLog(MessageEventArgs msg)
        {
            var file = $"{msg.Source}.log";

            try
            {
                var entry = $"[{DateTime.Now}] <{msg.Nick}> {msg.Message}";
                Log.Debug($"{file}: {entry}");

                _fileSystem.AppendText($"{_path}/{file}", entry);
            }
            catch (Exception e)
            {
                Log.Error($"Error writing to {msg.Source}.log: {e.Message}");
            }
        }
    }
}