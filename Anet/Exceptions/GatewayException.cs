using System;

namespace Anet
{
    public class GatewayException : Exception
    {
        public GatewayException(string url, string response, string message = null)
            : base(message)
        {
            Url = url;
            Response = response;
        }

        public string Url { get; set; }

        public string Response { get; set; }
    }
}
