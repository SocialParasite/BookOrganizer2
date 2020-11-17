using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;

namespace BookOrganizer2.Domain.BookProfile
{
    public class ReadDateId : ValueObject
    {
        public Guid Value { get; private set; }

        public ReadDateId()
        {
            
        }
        public ReadDateId(Guid id)
        {
            if (id == default)
                throw new ArgumentException("Invalid id!", nameof(id));

            Value = id;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(ReadDateId self) => self.Value;

        public static implicit operator ReadDateId(Guid value)
            => new ReadDateId(new SequentialGuid(value));
    }
}

