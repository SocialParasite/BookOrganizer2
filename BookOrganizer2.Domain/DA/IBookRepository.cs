using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.PublisherProfile;

namespace BookOrganizer2.Domain.DA
{
    public interface IBookRepository
    {
        Task<Book> LoadAsync(BookId id);
        Task ChangeLanguage(Book book, LanguageId languageId);
        Task ChangePublisher(Book book, PublisherId publisherId);
        Task ChangeAuthors(Book book, ICollection<Author> authors);
        Task ChangeGenres(Book book, ICollection<Genre> genres);
        Task ChangeFormats(Book book, ICollection<Format> formats);
        Task ChangeReadDates(Book book, ICollection<BookReadDate> bookReadDates);

        ValueTask<Author> GetAuthorAsync(AuthorId authorId);
        ValueTask<Publisher> GetPublisherAsync(PublisherId publisherId);
        ValueTask<Format> GetFormatAsync(FormatId formatId);
        ValueTask<Language> GetLanguageAsync(LanguageId languageId);
        ValueTask<Genre> GetGenreAsync(GenreId genreId);
    }
}
