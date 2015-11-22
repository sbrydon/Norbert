using System;

namespace Norbert.Cli.Exceptions
{
    public class LoadModuleException : Exception
    {
        public LoadModuleException(string file) : base($"Error loading {file}")
        {
        }
    }
}