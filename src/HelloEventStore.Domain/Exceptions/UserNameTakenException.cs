using System;

namespace HelloEventStore.Domain.Exceptions
{
    public class UserNameTakenException : Exception
    {
        public UserNameTakenException(string message) : base(message)
        { }
    }
}