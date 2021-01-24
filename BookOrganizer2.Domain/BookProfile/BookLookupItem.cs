using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.BookProfile
{
    public class BookLookupItem : LookupItem
    {
        public BookStatus BookStatus { get; init; }
    }
}