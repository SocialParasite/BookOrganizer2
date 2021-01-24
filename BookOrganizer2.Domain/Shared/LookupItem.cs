using System;

namespace BookOrganizer2.Domain.Shared
{
    public class LookupItem
    {
        public Guid Id { get; init; }
        public string DisplayMember { get; init; }
        public string Picture { get; init; }
        public string ViewModelName { get; init; }
        public string InfoText { get; init; }
    }
}
