using System;
using ChatSharp;
using Norbert.Cli.Exceptions;

namespace Norbert.Cli
{
    class Program
    {
        private static Config _config;

        static void Main(string[] args)
        {
            try
            {
                _config = Config.Load();
            }
            catch (LoadConfigException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }

            var client = new IrcClient(_config.Server, new IrcUser(_config.Nick, _config.User));
            client.ConnectionComplete += delegate
            {
                Console.WriteLine("Connected, joining channels..");

                foreach (var channel in _config.Channels)
                {
                    client.JoinChannel(channel);
                    Console.WriteLine("- joined {0}", channel);
                }

                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("Press any key to exit");
            };

            Console.WriteLine("Connecting to {0}..", _config.Server);
            client.ConnectAsync();

            Console.ReadKey();
            client.Quit(_config.QuitMsg);
        }
    }
}
