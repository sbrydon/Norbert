using System;

namespace Norbert.Modules.Common.Exceptions
{
    public class LoadConfigException : Exception
    {
        public LoadConfigException(string file, string message) :
            base($"Error loading '{file}': {message}")
        {
        }
    }
}