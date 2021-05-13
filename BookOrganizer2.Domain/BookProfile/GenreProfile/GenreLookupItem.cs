using System;

namespace BookOrganizer2.Domain.BookProfile.GenreProfile
{
    public class GenreLookupItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
