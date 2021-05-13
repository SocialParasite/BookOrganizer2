using System;

namespace BookOrganizer2.Domain.BookProfile.FormatProfile
{
    public class FormatLookupItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
