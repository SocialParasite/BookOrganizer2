using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.Common
{
    public class NoteId : ValueObject
    {
        public Guid Value { get; private set; }

        public NoteId() { }

        public NoteId(Guid id)
        {
            if (id == default)
                throw new ArgumentException("Invalid id!", nameof(id));

            Value = id;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(NoteId self) => self.Value;

        public static implicit operator NoteId(Guid value)
            => new(new SequentialGuid(value));
    }
}

