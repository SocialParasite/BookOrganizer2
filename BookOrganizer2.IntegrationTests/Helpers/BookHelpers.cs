using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.DA.Repositories;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.Domain.Shared;
using Commands = BookOrganizer2.Domain.BookProfile.Commands;
using Events = BookOrganizer2.Domain.BookProfile.Events;

namespace BookOrganizer2.IntegrationTests.Helpers
{
    public static class BookHelpers
    {
        public static async Task<Book> CreateValidBook(string title = null)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);

            var command = new Commands.Create
            {
                Id = new BookId(SequentialGuid.NewSequentialGuid()),
                Title = title ?? "Book 1"

            };

            await bookService.Handle(command);
            return await repository.GetAsync(command.Id);
        }

        public static async Task<Book> CreateValidBookWithAllProperties()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);

            var language = await LanguageHelpers.CreateValidLanguage();
            var publisher = await PublisherHelpers.CreateValidPublisher();

            var author1 = await AuthorHelpers.CreateValidAuthor();
            var author2 = await AuthorHelpers.CreateValidAuthor();
            var authors = new List<Author> { author1, author2 };

            var format1 = await FormatHelpers.CreateValidFormat();
            var format2 = await FormatHelpers.CreateValidFormat();
            var formats = new List<Format> { format1, format2 };

            var genre1 = await GenreHelpers.CreateValidGenre();
            var genre2 = await GenreHelpers.CreateValidGenre();
            var genres = new List<Genre> { genre1, genre2 };

            var bookReadDates = new List<BookReadDate> { new BookReadDate(DateTime.Now), new BookReadDate(DateTime.Now) };

            var command = new Commands.Create
            {
                Id = new BookId(SequentialGuid.NewSequentialGuid()),
                Title = "Book 1",
                ReleaseYear = 2019,
                PageCount = 123,
                WordCount = 61_500,
                Isbn = "9781566199094",
                BookCoverPath = @"C:\temp\pic.jpg",
                Description = "description",
                Notes = "notes",
                IsRead = true,
                Language = language,
                Publisher = publisher,
                Authors = authors,
                BookReadDates = bookReadDates,
                Formats = formats,
                Genres = genres
            };

            await bookService.Handle(command);
            return await repository.GetAsync(command.Id);
        }

        //internal static Task UpdateBook(Book sut)
        //{
        //    var connectionString = ConnectivityService.GetConnectionString("TEMP");
        //    var context = new BookOrganizer2DbContext(connectionString);
        //    var repository = new BookRepository(context);

        //    var bookService = new BookService(repository);
        //    var command = new Commands.Update
        //    {
        //        Id = sut.Id,
        //        Title
        //        Notes = sut.Notes
        //    };

        //    return bookService.Handle(command);
        //}

        public static Task CreateInvalidBook()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);
            var bookService = new BookService(repository);

            var bookId = new BookId(SequentialGuid.NewSequentialGuid());
            var command = new Commands.Create { Id = bookId };

            return bookService.Handle(command);
        }

        public static Task UpdateBookTitle(BookId id, string newTitle)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetTitle
            {
                Id = id,
                Title = newTitle
            };

            return bookService.Handle(command);
        }

        public static Task UpdateReleaseYear(BookId id, int year)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetReleaseYear
            {
                Id = id,
                ReleaseYear = year
            };

            return bookService.Handle(command);
        }

        public static Task UpdatePageCount(BookId id, int pCount)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetPageCount
            {
                Id = id,
                PageCount = pCount
            };

            return bookService.Handle(command);
        }

        public static Task UpdateWordCount(BookId id, int wCount)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetWordCount
            {
                Id = id,
                WordCount = wCount
            };

            return bookService.Handle(command);
        }

        public static Task UpdateIsbn(BookId id, string isbn)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetIsbn
            {
                Id = id,
                Isbn = isbn
            };

            return bookService.Handle(command);
        }

        public static Task UpdateCoverPath(BookId id, string path)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetBookCoverPath
            {
                Id = id,
                BookCoverPath = @"\\filepath\newFile.jpg"
            };

            return bookService.Handle(command);
        }

        public static Task UpdateDescription(BookId id, string description)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetDescription
            {
                Id = id,
                Description = description
            };

            return bookService.Handle(command);
        }

        public static Task UpdateNotes(BookId id, string notes)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetNotes
            {
                Id = id,
                Notes = notes
            };

            return bookService.Handle(command);
        }

        public static Task UpdateReadStatus(BookId id, bool status)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetIsRead
            {
                Id = id,
                IsRead = false
            };

            return bookService.Handle(command);
        }

        public static Task UpdateLanguage(BookId bookId, Language language)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetLanguage
            {
                Id = bookId,
                Language = language
            };

            return bookService.Handle(command);
        }

        public static Task UpdatePublisher(BookId bookId, Publisher publisher)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetPublisher
            {
                Id = bookId,
                Publisher = publisher
            };

            return bookService.Handle(command);
        }

        public static Task UpdateAuthors(BookId bookId, List<Author> authors)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetAuthors
            {
                Id = bookId,
                Authors = authors
            };

            return bookService.Handle(command);
        }

        public static Task UpdateFormats(BookId bookId, List<Format> formats)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetFormats
            {
                Id = bookId,
                Formats = formats
            };

            return bookService.Handle(command);
        }

        public static Task UpdateGenres(BookId bookId, List<Genre> genres)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetGenres
            {
                Id = bookId,
                Genres = genres
            };

            return bookService.Handle(command);
        }

        public static Task UpdateReadDates(BookId bookId, List<BookReadDate> readDates)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.SetBookReadDates
            {
                Id = bookId,
                BookReadDates = readDates
            };

            return bookService.Handle(command);
        }
        // DELETE
        public static Task RemoveBook(BookId id)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new BookRepository(context);

            var bookService = new BookService(repository);
            var command = new Commands.DeleteBook
            {
                Id = id,
            };

            return bookService.Handle(command);
        }
    }
}
