using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.PublisherProfile;
using Microsoft.EntityFrameworkCore;
using Language = BookOrganizer2.Domain.BookProfile.LanguageProfile.Language;

namespace BookOrganizer2.DA.Repositories
{
    public class BookRepository : BaseRepository<Book, BookOrganizer2DbContext, BookId>, IBookRepository
    {
        public BookRepository(BookOrganizer2DbContext context) : base(context)
        {
        }

        public async Task<Book> LoadAsync(BookId id)
        {
            if (id != default)
            {
                return await Context.Books
                    .Include(b => b.Language)
                    .Include(b => b.Publisher)
                    .Include(b => b.Authors)
                    .Include(b => b.Formats)
                    .Include(b => b.Genres)
                    .Include(b => b.ReadDates)
                    .Include(b => b.Series)
                    .ThenInclude(s => s.Series)
                    .ThenInclude(s => s.Books)
                    .ThenInclude(b => b.Book)
                    //.Include(b => b.Series)
                    //.ThenInclude(s => s.Book)
                    .FirstOrDefaultAsync(b => b.Id == id)
                    .ConfigureAwait(false);
            }

            return Book.NewBook;
        }
        public async Task ChangeLanguage(Book a, LanguageId languageId)
        {
            var book = await Context.Books.FindAsync(a.Id).ConfigureAwait(false);
            var language = await GetLanguageAsync(languageId).ConfigureAwait(false);
            book.SetLanguage(language);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ChangePublisher(Book a, PublisherId publisherId)
        {
            var book = await Context.Books.FindAsync(a.Id).ConfigureAwait(false);
            var publisher = await GetPublisherAsync(publisherId).ConfigureAwait(false);
            book.SetPublisher(publisher);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ChangeAuthors(Book book, ICollection<Author> authors)
        {
            var b = await Context.Books.FindAsync(book.Id).ConfigureAwait(false);

            var newAuthors = new List<Author>();
            foreach (var author in authors)
            {
                var a = await GetAuthorAsync(author.Id).ConfigureAwait(false);
                newAuthors.Add(a);
            }
            
            b.SetAuthors(newAuthors);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ChangeGenres(Book book, ICollection<Genre> genres)
        {
            var b = await Context.Books.FindAsync(book.Id).ConfigureAwait(false);

            var newGenres = new List<Genre>();
            foreach (var genre in genres)
            {
                var g = await GetGenreAsync(genre.Id).ConfigureAwait(false);
                newGenres.Add(g);
            }

            b.SetGenres(newGenres);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ChangeFormats(Book book, ICollection<Format> formats)
        {
            var b = await Context.Books.FindAsync(book.Id).ConfigureAwait(false);

            var newFormats = new List<Format>();
            foreach (var format in formats)
            {
                var f = await GetFormatAsync(format.Id).ConfigureAwait(false);
                newFormats.Add(f);
            }


            b.SetFormats(newFormats);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ChangeReadDates(Book book, ICollection<BookReadDate> bookReadDates)
        {
            var b = await Context.Books.FindAsync(book.Id).ConfigureAwait(false);

            b.SetReadDates(bookReadDates);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public override async Task RemoveAsync(BookId id)
        {
            var entity = await LoadAsync(id).ConfigureAwait(false);

            if (entity != null)
                Context.Remove(entity);
        }

        private async Task<Language> GetLanguageAsync(LanguageId languageId) 
            => await Context.Languages.FindAsync(languageId).ConfigureAwait(false);

        private async Task<Publisher> GetPublisherAsync(PublisherId publisherId)
            => await Context.Publishers.FindAsync(publisherId).ConfigureAwait(false);

        public async Task<Author> GetAuthorAsync(AuthorId authorId)
            => await Context.Authors.FindAsync(authorId).ConfigureAwait(false);

        public async Task<Format> GetFormatAsync(FormatId formatId)
            => await Context.Formats.FindAsync(formatId).ConfigureAwait(false);

        public async Task<Genre> GetGenreAsync(GenreId genreId)
            => await Context.Genres.FindAsync(genreId).ConfigureAwait(false);

        private async Task<Series> GetSeriesAsync(SeriesId seriesId)
            => await Context.Series.FindAsync(seriesId).ConfigureAwait(false);

    }
}
