using System;
using System.Threading.Tasks;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.DA;

namespace BookOrganizer2.Domain.Services
{
    public interface IBookDomainService : IDomainService<Book, BookId>
    {
        Task<Book> LoadAsync(BookId id) => ((IBookRepository)Repository).LoadAsync(id);

        Task<Author> GetAuthorAsync(Guid id) => ((IBookRepository)Repository).GetAuthorAsync(id);
        Task<Format> GetFormatAsync(Guid id) => ((IBookRepository)Repository).GetFormatAsync(id);
        Task<Genre> GetGenreAsync(Guid id) => ((IBookRepository)Repository).GetGenreAsync(id);

        Task<Genre> AddNewGenre(string name);
        Task<Format> AddNewFormat(string name);
    }
}
