using System;

namespace BookOrganizer2.Domain.Exceptions
{
    public sealed class InvalidTitleException : Exception
    {
        public InvalidTitleException() { }

        public InvalidTitleException(string message) : base(message) { }
    }
}
