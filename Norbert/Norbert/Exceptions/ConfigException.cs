using System;

namespace Norbert.Exceptions
{
    public class ConfigException : Exception
    {
        public ConfigException(string message) : base($"Error loading App.config: {message}")
        {
        }
    }
}