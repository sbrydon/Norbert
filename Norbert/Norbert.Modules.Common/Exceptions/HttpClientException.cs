using System;

namespace Norbert.Modules.Common.Exceptions
{
    public class HttpClientException : Exception
    {
        public HttpClientException(string uri, string message) :
            base($"Error requesting '{uri}': {message}")
        {
        }
    }
}