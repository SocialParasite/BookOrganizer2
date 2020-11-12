using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.BookProfile.FormatProfile
{
    public class FormatId : ValueObject
    {
        public Guid Value { get; private set; }

        public FormatId(Guid id)
        {
            if (id == default)
                throw new ArgumentException("Invalid id!", nameof(id));

            Value = id;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(FormatId self) => self.Value;

        public static implicit operator FormatId(Guid value)
            => new FormatId(new SequentialGuid(value));
    }
}