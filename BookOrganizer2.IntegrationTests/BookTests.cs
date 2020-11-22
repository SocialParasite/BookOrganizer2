using System;
using System.Threading.Tasks;
using BookOrganizer2.DA.Repositories;
using BookOrganizer2.IntegrationTests.Helpers;
using FluentAssertions;
using Xunit;

namespace BookOrganizer2.IntegrationTests
{
    public sealed partial class DatabaseTests
    {
        [Fact]
        public async Task Book_inserted_to_database()
        {
            var book = await BookHelpers.CreateValidBook();
            var repository = new BookRepository(_fixture.Context);

            (await repository.ExistsAsync(book.Id)).Should().BeTrue();
        }

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
            book.Notes.Should().Be("notes");
            book.IsRead.Should().BeTrue();

            book.Language.Should().NotBeNull();
            book.Publisher.Should().NotBeNull();
            book.Authors.Count.Should().Be(2);
            book.Formats.Count.Should().Be(2);
            book.Genres.Count.Should().Be(2);
            book.ReadDates.Count.Should().Be(2);
        }

        [Fact]
        public void Invalid_Book()
        {
            Func<Task> action = async () => await BookHelpers.CreateInvalidBook();
            action.Should().ThrowAsync<ArgumentException>();
        }

        //[Fact]
        //public async Task Update_Book()
        //{
        //    var book = await BookHelpers.CreateValidBook();
        //    book.LastName.Should().Be("Rothfuss");

        //    var sut = Book.Create(book.Id,
        //        "Scott", "Lynch",
        //        new DateTime(1978, 4, 2),
        //        "bio", @"\\pics\scott.jpg", "notes");

        //    await BookHelpers.UpdateBook(sut);

        //    await _fixture.Context.Entry(book).ReloadAsync();

        //    book.LastName.Should().Be("Lynch");
        //}

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

        [Fact]
        public async Task Update_Book_Notes()
        {
            var book = await BookHelpers.CreateValidBookWithAllProperties();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            var sut = await repository.GetAsync(book.Id);

            var bookId = sut.Id;

            sut.Should().NotBeNull();
            sut.Notes.Should().Be("notes");

            await BookHelpers.UpdateNotes(sut.Id, "You can always wish...");

            await _fixture.Context.Entry(sut).ReloadAsync();

            sut.Notes.Should().Contain("always wish");
            sut.Id.Should().Be(bookId);
        }

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

        // TODO: Collections

        [Fact]
        public async Task Remove_Book()
        {
            // TODO: Book with all props filled removal
            var book = await BookHelpers.CreateValidBook();

            var repository = new BookRepository(_fixture.Context);
            (await repository.ExistsAsync(book.Id)).Should().BeTrue();

            await BookHelpers.RemoveBook(book.Id);

            var sut = await repository.GetAsync(book.Id);
            await _fixture.Context.Entry(sut).ReloadAsync();
            (await repository.ExistsAsync(book.Id)).Should().BeFalse();
        }
    }
}