using System;

namespace HelloEventStore.Domain.Exceptions
{
    public class OrderStateException : Exception
    {
        public OrderStateException(string message)
            : base(message)
        {
        }
    }
}