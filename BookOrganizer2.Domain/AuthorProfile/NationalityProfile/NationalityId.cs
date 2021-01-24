using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.AuthorProfile.NationalityProfile
{
    public class NationalityId : ValueObject
    {
        public Guid Value { get; private set; }

        public NationalityId(Guid id)
        {
            if (id == default)
                throw new ArgumentException("Invalid id!", nameof(id));

            Value = id;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(NationalityId self) => self.Value;

        public static implicit operator NationalityId(Guid value)
            => new(new SequentialGuid(value));
    }
}