using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.PublisherProfile
{
    public class PublisherId : ValueObject
    {
        public Guid Value { get; private set; }

        public PublisherId(Guid id)
        {
            if (id == default)
                throw new ArgumentException("Invalid id!", nameof(id));

            Value = id;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(PublisherId self) => self.Value;

        public static implicit operator PublisherId(Guid value)
            => new(new SequentialGuid(value));
    }
}
