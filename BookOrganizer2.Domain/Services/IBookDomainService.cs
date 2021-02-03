using System;
using System.Threading.Tasks;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.PublisherProfile;

namespace BookOrganizer2.Domain.Services
{
    public interface IBookDomainService : IDomainService<Book, BookId>
    {
        Task<Book> LoadAsync(BookId id) => ((IBookRepository)Repository).LoadAsync(id);

        ValueTask<Author> GetAuthorAsync(Guid id) => ((IBookRepository)Repository).GetAuthorAsync(id);
        ValueTask<Format> GetFormatAsync(Guid id) => ((IBookRepository)Repository).GetFormatAsync(id);
        ValueTask<Genre> GetGenreAsync(Guid id) => ((IBookRepository)Repository).GetGenreAsync(id);
        ValueTask<Publisher> GetPublisherAsync(Guid id) => ((IBookRepository)Repository).GetPublisherAsync(id);
        ValueTask<Language> GetLanguageAsync(Guid id) => ((IBookRepository)Repository).GetLanguageAsync(id);

        Task<Genre> AddNewGenre(string name);
        Task<Format> AddNewFormat(string name);
    }
}
