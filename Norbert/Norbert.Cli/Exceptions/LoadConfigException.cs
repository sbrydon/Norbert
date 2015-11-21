using System;

namespace Norbert.Cli.Exceptions
{
    public class LoadConfigException : Exception
    {
        public LoadConfigException(string message) : base($"Error loading App.config: {message}")
        {
        }
    }
}