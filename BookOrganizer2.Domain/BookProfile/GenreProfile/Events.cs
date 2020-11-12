using System;

namespace BookOrganizer2.Domain.BookProfile.GenreProfile
{
    public static class Events
    {
        public class GenreCreated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class GenreUpdated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class GenreDeleted
        {
            public Guid Id { get; set; }
        }
    }
}
