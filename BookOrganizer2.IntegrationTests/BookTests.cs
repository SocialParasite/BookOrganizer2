using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.DA.Repositories;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.IntegrationTests.Helpers;
using FluentAssertions;
using Xunit;

namespace BookOrganizer2.IntegrationTests
{
    public sealed partial class DatabaseTests
    {
        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Book_inserted_to_database()
        {
            var book = await BookHelpers.CreateValidBook();
            var repository = new BookRepository(_fixture.Context);

            (await repository.ExistsAsync(book.Id)).Should().BeTrue();
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Book_with_all_properties_inserted_to_database()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();
            var repository = new BookRepository(_fixture.Context);

            (await repository.ExistsAsync(book.Id)).Should().BeTrue();
            book = await repository.LoadAsync(book.Id);

            book.Title.Should().Be("Book 1");
            book.ReleaseYear.Should().Be(2019);
            book.PageCount.Should().Be(123);
            book.WordCount.Should().Be(61_500);
            book.Isbn.Should().Be("9781566199094");
            book.BookCoverPath.Should().Be(@"C:\temp\pic.jpg");
            book.Description.Should().Be("description");
            book.NotesOld.Should().Be("notes");
            book.IsRead.Should().BeTrue();

            book.Language.Should().NotBeNull();
            book.Publisher.Should().NotBeNull();
            book.Authors.Count.Should().Be(2);
            book.Formats.Count.Should().Be(2);
            book.Genres.Count.Should().Be(2);
            book.ReadDates.Count.Should().Be(2);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public void Invalid_Book()
        {
            Func<Task> action = async () => await BookHelpers.CreateInvalidBook();
            action.Should().ThrowAsync<ArgumentException>();
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book()
        {
            var book = await BookHelpers.CreateValidBook();
            book.Title.Should().Be("Book 1");

            // TODO: Test collections

            // TODO: Update all props, replace collections
            // TODO: Test that collections count is not higher than previously, but items are not the same
            //await BookHelpers.UpdateBook(sut);

            //await _fixture.Context.Entry(book).ReloadAsync();

            //book.Title.Should().Be("Lynch");
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_Title()
        {
            var book = await BookHelpers.CreateValidBook();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.Title.Should().Be("Book 1");

            await BookHelpers.UpdateBookTitle(sut.Id, "Book 1 Limited Edition");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Title.Should().Be("Book 1 Limited Edition");
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_ReleaseYear()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.ReleaseYear.Should().Be(2019);

            await BookHelpers.UpdateReleaseYear(sut.Id, 1920);

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.ReleaseYear.Should().Be(1920);
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_PageCount()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.PageCount.Should().Be(123);

            await BookHelpers.UpdatePageCount(sut.Id, 321);

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.PageCount.Should().Be(321);
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_WordCount()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.WordCount.Should().Be(61_500);

            await BookHelpers.UpdateWordCount(sut.Id, 150_600);

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.WordCount.Should().Be(150_600);
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_Isbn()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.Isbn.Should().Be("9781566199094");

            await BookHelpers.UpdateIsbn(sut.Id, "000224585X");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Isbn.Should().Be("000224585X");
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_CoverPath()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.BookCoverPath.Should().Be(@"C:\temp\pic.jpg");

            await BookHelpers.UpdateCoverPath(sut.Id, @"\\filepath\newFile.jpg");
            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.BookCoverPath.Should().Be(@"\\filepath\newFile.jpg");
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_Description()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.Description.Should().Be("description");

            await BookHelpers.UpdateDescription(sut.Id, "Could I please have book number three?");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Description.Should().Contain("Could I please");
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_Notes()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.NotesOld.Should().Be("notes");

            await BookHelpers.UpdateNotes(sut.Id, "You can always wish...");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.NotesOld.Should().Contain("always wish");
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_ReadStatus()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.IsRead.Should().BeTrue();

            await BookHelpers.UpdateReadStatus(sut.Id, false);

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.IsRead.Should().BeFalse();
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_Language()
        {
            var book = await BookHelpers.CreateValidBook();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.LoadAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.Language.Should().BeNull();

            var language = await LanguageHelpers.CreateValidLanguage();
            await BookHelpers.UpdateLanguage(sut.Id, language);

            sut = await repository.LoadAsync(book.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Language.Id.Should().Be(language.Id);
            sut.Language.Name.Should().Be(language.Name);
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_Publisher()
        {
            var book = await BookHelpers.CreateValidBook();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.LoadAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.Publisher.Should().BeNull();

            var publisher = await PublisherHelpers.CreateValidPublisher();
            await BookHelpers.UpdatePublisher(sut.Id, publisher);

            sut = await repository.LoadAsync(book.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Publisher.Id.Should().Be(publisher.Id);
            sut.Publisher.Name.Should().Be(publisher.Name);
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_Authors()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.LoadAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.Authors.Count.Should().Be(2);

            var author1 = await AuthorHelpers.CreateValidAuthor();
            var author2 = await AuthorHelpers.CreateValidAuthor();
            var authors = new List<Author> {author1, author2};
            await BookHelpers.UpdateAuthors(sut.Id, authors);

            sut = await repository.LoadAsync(book.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Authors.Count.Should().Be(4);
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_Formats()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.LoadAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.Formats.Count.Should().Be(2);

            var format1 = await FormatHelpers.CreateValidFormat();
            var format2 = await FormatHelpers.CreateValidFormat();
            var formats = new List<Format> { format1, format2 };
            await BookHelpers.UpdateFormats(sut.Id, formats);

            sut = await repository.LoadAsync(book.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Formats.Count.Should().Be(4);
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_Genres()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.LoadAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.Genres.Count.Should().Be(2);

            var genre1 = await GenreHelpers.CreateValidGenre();
            var genre2 = await GenreHelpers.CreateValidGenre();
            var genres = new List<Genre> { genre1, genre2 };
            await BookHelpers.UpdateGenres(sut.Id, genres);

            sut = await repository.LoadAsync(book.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Genres.Count.Should().Be(4);
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Update_Book_Read_Dates()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.LoadAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.ReadDates.Count.Should().Be(2);

            var readDate1 = new BookReadDate(DateTime.Now);
            var readDate2 = new BookReadDate(DateTime.Now);
            var readDates = new List<BookReadDate> { readDate1, readDate2 };
            await BookHelpers.UpdateReadDates(sut.Id, readDates);

            sut = await repository.LoadAsync(book.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.ReadDates.Count.Should().Be(4);
            sut.Id.Should().Be(bookId);
        }

        [Trait("Integration", "DB\\Book")]
        [Fact]
        public async Task Remove_Book()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            await BookHelpers.RemoveBook(book.Id);

            var sut = await repository.GetAsync(book.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();
            (await repository.ExistsAsync(book.Id)).Should().BeFalse();
        }
    }
}