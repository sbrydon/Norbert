using System;
using log4net;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.ChatLog
{
    public class ChatLogModule : INorbertModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChatLogModule));
        private IConfigLoader _configLoader;
        private IFileSystem _fileSystem;
        private IChatClient _client;
        private string _path;

        public void Loaded(IConfigLoader configLoader, IFileSystem fileSystem, IChatClient client)
        {
            _configLoader = configLoader;
            _fileSystem = fileSystem;
            _client = client;

            _client.MessageReceived += OnMessageReceived;
            SetupPath();
        }

        public void Unloaded()
        {
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs eventArgs)
        {
            AppendToLog(eventArgs);
        }

        private void SetupPath()
        {
            var config = new Config();
            try
            {
                config = _configLoader.Load<Config>("ChatLog/Config.json");
            }
            catch (LoadConfigException e)
            {
                Log.Error(e.Message);
            }

            if (string.IsNullOrWhiteSpace(config.Path))
            {
                _path = "ChatLogs";
                Log.Info("No path specified, using 'ChatLogs'");
            }
            else
            {
                _path = config.Path;
            }

            try
            {
                if (_fileSystem.DirectoryExists(_path))
                {
                    Log.Info("Path exists, skipping creation");
                }
                else
                {
                    Log.Info("Path does not exist, creating it..");
                    _fileSystem.CreateDirectory(_path);
                }

                Log.Info($"Path is '{_path}'");
            }
            catch (Exception e)
            {
                Log.Error($"Error creating '{_path}': {e.Message}");
                Log.Error("Path not set");
            }
        }

        private void AppendToLog(MessageReceivedEventArgs msg)
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
