using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.BookProfile.SeriesProfile
{
    public class SeriesId : ValueObject
    {
        public Guid Value { get; private set; }

        public SeriesId() { }

        public SeriesId(Guid id)
        {
            if (id == default)
                throw new ArgumentException("Invalid id!", nameof(id));

            Value = id;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(SeriesId self) => self.Value;

        public static implicit operator SeriesId(Guid value)
            => new SeriesId(new SequentialGuid(value));
    }
}