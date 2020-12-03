using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;

namespace BookOrganizer2.Domain.BookProfile
{
    public class BookId : ValueObject
    {
        public Guid Value { get; private set; }

        public BookId() { }

        public BookId(Guid id)
        {
            if (id == default)
                throw new ArgumentException("Invalid id!", nameof(id));

            Value = id;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(BookId self) => self.Value;

        public static implicit operator BookId(Guid value)
            => new BookId(new SequentialGuid(value));
    }
}

