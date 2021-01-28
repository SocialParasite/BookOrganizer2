using BookOrganizer2.Domain.BookProfile.SeriesProfile;

namespace BookOrganizer2.Domain.BookProfile
{
    public class ReadOrder
    {
        public BookId BooksId { get; set; }
        public SeriesId SeriesId { get; set; }
        public int Instalment { get; set; }

        public Book Book { get; set; }
        public Series Series { get; set; }

        public static ReadOrder NewReadOrder(Book book, Series series, int instalment)
            => new() { Book = book, Series = series, Instalment = instalment };
    }
}
