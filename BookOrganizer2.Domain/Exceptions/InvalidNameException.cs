using System;

namespace BookOrganizer2.Domain.Exceptions
{
    public sealed class InvalidNameException : Exception
    {
        public InvalidNameException() { }

        public InvalidNameException(string message) : base(message) { }
    }
}
