using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using static BookOrganizer2.Domain.BookProfile.Commands;

namespace BookOrganizer2.Domain.BookProfile
{
    public class BookService : IDomainService<Book, BookId>
    {
        private readonly ILanguageLookupDataService _languageLookupDataService;
        private readonly IPublisherLookupDataService _publisherLookupDataService;
        private readonly IAuthorLookupDataService _authorLookupDataService;
        private readonly IFormatLookupDataService _formatLookupDataService;
        private readonly IGenreLookupDataService _genreLookupDataService;

        public IRepository<Book, BookId> Repository { get; }

        public BookService(IRepository<Book, BookId> repository,
            ILanguageLookupDataService languageLookupDataService = null,
            IPublisherLookupDataService publisherLookupDataService = null,
            IAuthorLookupDataService authorLookupDataService = null,
            IFormatLookupDataService formatLookupDataService = null,
            IGenreLookupDataService genreLookupDataService = null)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _languageLookupDataService = languageLookupDataService;
            _publisherLookupDataService = publisherLookupDataService;
            _authorLookupDataService = authorLookupDataService;
            _formatLookupDataService = formatLookupDataService;
            _genreLookupDataService = genreLookupDataService;
        }

        public Book CreateItem()
            => Book.NewBook;

        public Task Handle(object command)
        {
            return command switch
            {
                Create cmd => HandleCreate(cmd),
                Update cmd => HandleFullUpdate(cmd),
                SetTitle cmd => HandleUpdate(cmd.Id, (a) => a.SetTitle(cmd.Title),
                    (a) => Repository.Update(a)),
                SetReleaseYear cmd => HandleUpdate(cmd.Id, (a) => a.SetReleaseYear(cmd.ReleaseYear),
                    (a) => Repository.Update(a)),
                SetPageCount cmd => HandleUpdate(cmd.Id, (a) => a.SetPageCount(cmd.PageCount),
                    (a) => Repository.Update(a)),
                SetWordCount cmd => HandleUpdate(cmd.Id, (a) => a.SetWordCount(cmd.WordCount),
                    (a) => Repository.Update(a)),
                SetIsbn cmd => HandleUpdate(cmd.Id, (a) => a.SetIsbn(cmd.Isbn),
                    (a) => Repository.Update(a)),
                SetBookCoverPath cmd => HandleUpdate(cmd.Id, (a) => a.SetBookCoverPath(cmd.BookCoverPath),
                    (a) => Repository.Update(a)),
                SetDescription cmd => HandleUpdate(cmd.Id, (a) => a.SetDescription(cmd.Description),
                    (a) => Repository.Update(a)),
                SetNotes cmd => HandleUpdate(cmd.Id, (a) => a.SetNotes(cmd.Notes),
                    (a) => Repository.Update(a)),
                SetIsRead cmd => HandleUpdate(cmd.Id, (a) => a.SetIsRead(cmd.IsRead),
                    (a) => Repository.Update(a)),

                SetLanguage cmd => HandleUpdateAsync(cmd.Id,
                    async a => await UpdateLanguageAsync(a, cmd.Language.Id)),
                SetPublisher cmd => HandleUpdateAsync(cmd.Id,
                    async a => await UpdatePublisherAsync(a, cmd.Publisher.Id)),

                SetAuthors cmd => HandleUpdateAsync(cmd.Id,
                    async a => await UpdateBookAuthorsAsync(a, cmd.Authors)),
                SetFormats cmd => HandleUpdateAsync(cmd.Id,
                    async a => await UpdateBookFormatsAsync(a, cmd.Formats)),
                SetGenres cmd => HandleUpdateAsync(cmd.Id,
                    async a => await UpdateBookGenresAsync(a, cmd.Genres)),
                SetSeries cmd => HandleUpdateAsync(cmd.Id,
                    async a => await UpdateBookSeriesAsync(a, cmd.Series)),
                SetBookReadDates cmd => HandleUpdateAsync(cmd.Id,
                    async a => await UpdateBookReadDatesAsync(a, cmd.BookReadDates)),

                DeleteBook cmd => HandleUpdateAsync(cmd.Id, _ => Repository.RemoveAsync(cmd.Id)),
                _ => Task.CompletedTask
            };
        }

        public Guid GetId(BookId id) => id?.Value ?? Guid.Empty;

        public async Task<Book> AddNew(Book model)
        {
            var command = new Create
            {
                Id = new BookId(SequentialGuid.NewSequentialGuid()),
                Title = model.Title,
                ReleaseYear = model.ReleaseYear,
                PageCount = model.PageCount,
                WordCount = model.WordCount,
                Isbn = model.Isbn,
                BookCoverPath = model.BookCoverPath,
                Description = model.Description,
                Notes = model.Notes,
                IsRead = model.IsRead,
                Language = model.Language,
                Publisher = model.Publisher,
                Authors = model.Authors,
                BookReadDates = model.ReadDates,
                Formats = model.Formats,
                Genres = model.Genres
            };

            await Handle(command);

            return await Repository.GetAsync(command.Id);
        }

        private async Task HandleCreate(Create cmd)
        {
            if (await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var book = Book.Create(cmd.Id,
                                   cmd.Title,
                                   cmd.ReleaseYear,
                                   cmd.PageCount,
                                   cmd.WordCount,
                                   cmd.Isbn,
                                   cmd.BookCoverPath,
                                   cmd.Description,
                                   cmd.Notes,
                                   cmd.IsRead);

            await Repository.AddAsync(book);

            if (cmd.Language is not null)
            {
                await UpdateLanguageAsync(book, cmd.Language.Id).ConfigureAwait(false);
            }

            if (cmd.Publisher is not null)
            {
                await UpdatePublisherAsync(book, cmd.Publisher.Id).ConfigureAwait(false);
            }

            if (cmd.Authors is not null)
            {
                await UpdateBookAuthorsAsync(book, cmd.Authors).ConfigureAwait(false);
            }

            if (cmd.BookReadDates is not null)
            {
                await UpdateBookReadDatesAsync(book, cmd.BookReadDates).ConfigureAwait(false);
            }

            if (cmd.Formats is not null)
            {
                await UpdateBookFormatsAsync(book, cmd.Formats).ConfigureAwait(false);
            }

            if (cmd.Genres is not null)
            {
                await UpdateBookGenresAsync(book, cmd.Genres).ConfigureAwait(false);
            }

            if (cmd.Authors is not null && cmd.Authors.Count > 0)
            {
                await UpdateBookAuthorsAsync(book, cmd.Authors).ConfigureAwait(false);
            }

            if (cmd.Formats is not null && cmd.Formats.Count > 0)
            {
                await UpdateBookFormatsAsync(book, cmd.Formats).ConfigureAwait(false);
            }

            if (cmd.Genres is not null && cmd.Genres.Count > 0)
            {
                await UpdateBookGenresAsync(book, cmd.Genres).ConfigureAwait(false);
            }

            if (cmd.Series is not null && cmd.Series.Count > 0)
            {
                await UpdateBookSeriesAsync(book, cmd.Series).ConfigureAwait(false);
            }

            if (cmd.BookReadDates is not null && cmd.BookReadDates.Count > 0)
            {
                await UpdateBookReadDatesAsync(book, cmd.BookReadDates).ConfigureAwait(false);
            }

            if (book.EnsureValidState())
            {
                await Repository.SaveAsync().ConfigureAwait(false);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        private async Task HandleFullUpdate(Update cmd)
        {
            if (!await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} was not found! Update cannot finish.");

            var updatableBook = await Repository.GetAsync(cmd.Id);

            //updatableBook.SetFirstName(cmd.FirstName);
            //updatableBook.SetLastName(cmd.LastName);
            //updatableBook.SetDateOfBirth(cmd.DateOfBirth);
            //updatableBook.SetBiography(cmd.Biography);
            //updatableBook.SetMugshotPath(cmd.MugshotPath);
            updatableBook.SetNotes(cmd.Notes);

            Repository.Update(updatableBook);

            if (updatableBook.EnsureValidState())
            {
                await Repository.SaveAsync();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        private async Task HandleUpdate(Guid id, Action<Book> operation, Action<Book> operation2 = null)
        {
            if (await Repository.ExistsAsync(id))
            {
                var book = await Repository.GetAsync(id);
                operation(book);
                operation2?.Invoke(book);

                if (book.EnsureValidState())
                {
                    await Repository.SaveAsync();
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private async Task HandleUpdateAsync(Guid bookId, Func<Book, Task> operation)
        {
            if (await Repository.ExistsAsync(bookId))
            {
                var book = await Repository.GetAsync(bookId);

                if (book is null)
                    throw new InvalidOperationException($"Entity with id {bookId} cannot be found");

                await operation(book);

                if (book.EnsureValidState())
                {
                    await Repository.SaveAsync();
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private Task UpdateLanguageAsync(Book book, LanguageId languageId) 
            => ((IBookRepository)Repository).ChangeLanguage(book, languageId);

        private Task UpdatePublisherAsync(Book book, PublisherId publisherId)
            => ((IBookRepository)Repository).ChangePublisher(book, publisherId);

        private Task UpdateBookAuthorsAsync(Book book, ICollection<Author> authors)
            => ((IBookRepository) Repository).ChangeAuthors(book, authors);

        private Task UpdateBookGenresAsync(Book book, ICollection<Genre> genres)
            => ((IBookRepository)Repository).ChangeGenres(book, genres);

        private Task UpdateBookFormatsAsync(Book book, ICollection<Format> formats)
            => ((IBookRepository)Repository).ChangeFormats(book, formats);

        private Task UpdateBookReadDatesAsync(Book book, ICollection<BookReadDate> bookReadDates)
            => ((IBookRepository)Repository).ChangeReadDates(book, bookReadDates);

        private Task UpdateBookSeriesAsync(Book book, ICollection<Series> series)
            => ((IBookRepository)Repository).ChangeSeries(book, series);
    }
}
