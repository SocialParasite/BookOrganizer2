using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.BookProfile.SeriesProfile
{
    public class SeriesLookupItem : LookupItem
    {
        public SeriesState SeriesState { get; init; }
    }

    public class SeriesState
    {
        public int BookCount { get; init; }

        public int ReadBookCount { get; init; }

        public int OwnedBookCount { get; init; }
    }
}
