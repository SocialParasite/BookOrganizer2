using System;

namespace BookOrganizer2.Domain.Shared
{
    public class LookupItem
    {
        public Guid Id { get; set; }
        public string DisplayMember { get; set; }
        public string Picture { get; set; }
        public string ViewModelName { get; set; }
        public object ItemStatus { get; set; }
    }
}
