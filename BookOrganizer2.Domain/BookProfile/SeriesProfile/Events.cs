using System;
using System.Collections.Generic;

namespace BookOrganizer2.Domain.BookProfile.SeriesProfile
{
    public static class Events
    {
        public class Created
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string PicturePath { get; set; }
            public string Description { get; set; }
            public ICollection<ReadOrder> Books { get; set; }
        }

        public class Updated
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string PicturePath { get; set; }
            public string Description { get; set; }
            public ICollection<ReadOrder> Books { get; set; }
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
            public ICollection<ReadOrder> Books { get; set; }
        }

        public class Deleted
        {
            public Guid Id { get; set; }
        }
    }
}
