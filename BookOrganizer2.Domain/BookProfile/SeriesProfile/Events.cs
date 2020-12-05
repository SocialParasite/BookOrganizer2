using System;
using System.Collections.Generic;

namespace BookOrganizer2.Domain.BookProfile.SeriesProfile
{
    public static class Events
    {
        public class SeriesCreated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string PicturePath { get; set; }
            public string Description { get; set; }
            public ICollection<Book> Books { get; set; }
        }

        public class SeriesUpdated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string PicturePath { get; set; }
            public string Description { get; set; }
            public ICollection<Book> Books { get; set; }
        }

        public class SeriesNameChanged
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class SeriesPicturePathChanged
        {
            public Guid Id { get; set; }
            public string PicturePath { get; set; }
        }

        public class SeriesDescriptionChanged
        {
            public Guid Id { get; set; }
            public string Description { get; set; }
        }

        public class BooksChanged
        {
            public Guid Id { get; set; }
            public ICollection<Book> Books { get; set; }
        }

        public class SeriesDeleted
        {
            public Guid Id { get; set; }
        }
    }
}
