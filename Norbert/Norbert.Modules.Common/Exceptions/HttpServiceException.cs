using System;

namespace Norbert.Modules.Common.Exceptions
{
    public class HttpServiceException : Exception
    {
        public HttpServiceException(string uri, string message) :
            base($"Error requesting '{uri}': {message}")
        {
        }
    }
}