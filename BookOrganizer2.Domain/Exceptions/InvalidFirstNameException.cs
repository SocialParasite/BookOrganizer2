using System;

namespace BookOrganizer2.Domain.Exceptions
{
    public sealed class InvalidFirstNameException : Exception
    {
        public InvalidFirstNameException() { }

        public InvalidFirstNameException(string message) : base(message) { }
    }
}
