using System;

namespace Norbert.Cli.Exceptions
{
    public class LoadModuleException : Exception
    {
        public LoadModuleException(string file, string message)
            : base($"Error loading '{file}': {message}")
        {
        }
    }
}