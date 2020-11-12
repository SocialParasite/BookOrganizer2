using System;

namespace BookOrganizer2.Domain.BookProfile.GenreProfile
{
    public static class Commands
    {
        public class Create
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Update
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class DeleteGenre
        {
            public Guid Id { get; set; }
        }
    }
}
