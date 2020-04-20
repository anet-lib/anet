using System;

namespace Anet
{
    public class ApiException : Exception
    {
        public ApiException(string url, string response, string message = null)
            : base(message)
        {
            Url = url;
            Response = response;
        }

        public string Url { get; set; }

        public string Response { get; set; }
    }
}
