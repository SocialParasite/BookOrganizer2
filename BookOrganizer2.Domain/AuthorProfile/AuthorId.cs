using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.AuthorProfile
{
    public class AuthorId : ValueObject
    {
        public Guid Value { get; private set; }

        public AuthorId(Guid id)
        {
            if (id == default)
                throw new ArgumentException("Invalid id!", nameof(id));

            Value = id;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(AuthorId self) => self.Value;

        public static implicit operator AuthorId(Guid value)
            => new AuthorId(new SequentialGuid(value));
    }
}
