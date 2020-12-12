using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.Domain.PublisherProfile;

namespace BookOrganizer2.Domain.DA
{
    public interface IBookRepository
    {
        Task ChangeLanguage(Book book, LanguageId languageId);
        Task ChangePublisher(Book book, PublisherId publisherId);
        Task ChangeAuthors(Book book, ICollection<Author> authors);
        Task ChangeGenres(Book book, ICollection<Genre> genres);
        Task ChangeFormats(Book book, ICollection<Format> formats);
        Task ChangeReadDates(Book book, ICollection<BookReadDate> bookReadDates);
        Task ChangeSeries(Book book, ICollection<ReadOrder> series);
    }
}
