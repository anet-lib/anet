using System;

namespace Anet
{
    public class GatewayException : Exception
    {
        public GatewayException(string url, string message = null)
            : base(message)
        {
            RequestUrl = url;
        }

        public string RequestUrl { get; }
    }
}
