using System;

namespace Anet
{
    public class GatewayException : Exception
    {
        public GatewayException(string message, Exception innerException = null)
            : base(message, innerException)
        { }
    }
}
