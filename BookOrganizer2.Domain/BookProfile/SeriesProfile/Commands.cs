using System;
using System.Collections.Generic;

namespace BookOrganizer2.Domain.BookProfile.SeriesProfile
{
    public static class Commands
    {
        public class Create
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string PicturePath { get; set; }
            public string Description { get; set; }
            public ICollection<ReadOrder> Books { get; set; }
        }

        public class Update
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string PicturePath { get; set; }
            public string Description { get; set; }
            public ICollection<ReadOrder> Books { get; set; }
        }
        public class SetSeriesName
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class SetPicturePath
        {
            public Guid Id { get; set; }
            public string PicturePath { get; set; }
        }

        public class SetDescription
        {
            public Guid Id { get; set; }
            public string Description { get; set; }
        }

        public class SetBooks
        {
            public Guid Id { get; set; }
            public ICollection<ReadOrder> Books { get; set; }
        }

        public class DeleteGenre
        {
            public Guid Id { get; set; }
        }
    }
}
