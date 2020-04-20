using System;

namespace Anet
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message = "Not found") : base(message)
        {
        }
    }
}
