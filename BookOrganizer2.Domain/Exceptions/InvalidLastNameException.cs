using System;

namespace BookOrganizer2.Domain.Exceptions
{
    public sealed class InvalidLastNameException : Exception
    {
        public InvalidLastNameException() { }

        public InvalidLastNameException(string message) : base(message) { }
    }
}
