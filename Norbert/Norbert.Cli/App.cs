using System;
using System.Configuration;
using ChatSharp;
using log4net;
using Norbert.Cli.Exceptions;

namespace Norbert.Cli
{
    class App
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(App));
        private static Config _config;
        private static ModuleManager _moduleManager;

        static void Main(string[] args)
        {
            Console.WriteLine($"Starting Norbert, press any key to exit {Environment.NewLine}");
            Log.Info("Norbert started");

            try
            {
                _config = new Config(ConfigurationManager.AppSettings);
            }
            catch (ConfigException e)
            {
                Log.Fatal(e.Message);
                Log.Info("Norbert ended");
                return;
            }

            var client = new IrcClient(_config.Server, new IrcUser(_config.Nick, _config.User));
            client.ConnectionComplete += (s, e) =>
            {
                Log.Info($"Connected to {_config.Server}, joining channels..");

                foreach (var channel in _config.Channels)
                {
                    client.JoinChannel(channel);
                    Log.Info($"Joined {_config.Server}/{channel}");
                }
            };

            _moduleManager = new ModuleManager(new ChatClient(client));
            try
            {
                _moduleManager.LoadModules();
            }
            catch (LoadModuleException e)
            {
                Log.Fatal(e.Message);
                Log.Info("Norbert ended");
                return;
            }

            Log.Info($"Connecting to {_config.Server}..");
            client.ConnectAsync();
            Console.ReadKey();

            client.Quit(_config.QuitMsg);
            Log.Info($"Disconnected from {_config.Server}, reason: Quit");
            Log.Info("Norbert ended");
        }
    }
}
