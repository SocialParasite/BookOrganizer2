using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.BookProfile.GenreProfile
{
    public class GenreId : ValueObject
    {
        public Guid Value { get; private set; }

        public GenreId(Guid id)
        {
            if (id == default)
                throw new ArgumentException("Invalid id!", nameof(id));

            Value = id;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(GenreId self) => self.Value;

        public static implicit operator GenreId(Guid value)
            => new GenreId(new SequentialGuid(value));
    }
}