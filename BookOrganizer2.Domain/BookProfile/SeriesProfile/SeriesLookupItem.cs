using System;

namespace BookOrganizer2.Domain.Shared
{
    public class SeriesLookupItem : LookupItem
    {
        public SeriesState SeriesState { get; set; }
    }

    public class SeriesState
    {
        public int BookCount { get; set; }

        public int ReadBookCount { get; set; }

        public int OwnedBookCount { get; set; }
    }
}
