using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.BookProfile.LanguageProfile
{
    public class LanguageId : ValueObject
    {
        public Guid Value { get; private set; }

        public LanguageId(Guid id)
        {
            if (id == default)
                throw new ArgumentException("Invalid id!", nameof(id));

            Value = id;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(LanguageId self) => self.Value;

        public static implicit operator LanguageId(Guid value)
            => new(new SequentialGuid(value));
    }
}