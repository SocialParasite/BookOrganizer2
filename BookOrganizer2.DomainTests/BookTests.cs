using System;
using System.Collections.Generic;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.Domain.Shared;
using FluentAssertions;
using Xunit;

namespace BookOrganizer2.DomainTests
{
    public class BookTests
    {
        private Book CreateBook()
            => Book.Create(new BookId(SequentialGuid.NewSequentialGuid()), "Nameless");

        [Fact]
        public void Book_with_all_information()
        {
            var language = Language.NewLanguage;
            var publisher = Publisher.NewPublisher;
            var authors = new List<Author> { Author.NewAuthor, Author.NewAuthor };
            var bookReadDates = new List<BookReadDate> { new BookReadDate(), new BookReadDate() };
            var formats = new List<Format> { Format.NewFormat, Format.NewFormat };
            var genres = new List<Genre> { Genre.NewGenre, Genre.NewGenre };

            var sut = Book.Create(new BookId(SequentialGuid.NewSequentialGuid()),
                                  "Nameless",
                                  2_000,
                                  123,
                                  61_500,
                                  "9781566199094",
                                  @"C:\temp\cover.jpg",
                                  "description",
                                  "notes",
                                  true,
                                  language,
                                  publisher,
                                  authors,
                                  bookReadDates,
                                  formats,
                                  genres);

            sut.Title.Should().Be("Nameless");
            sut.ReleaseYear.Should().Be(2_000);
            sut.PageCount.Should().Be(123);
            sut.WordCount.Should().Be(61_500);
            sut.Isbn.Should().Be("9781566199094");
            sut.BookCoverPath.Should().Be(@"C:\temp\cover.jpg");
            sut.Description.Should().Be("description");
            sut.Notes.Should().Be("notes");
            sut.IsRead.Should().BeTrue();
            sut.Language.Should().NotBeNull();
            sut.Language.Id.Should().Be(language.Id);
            sut.Publisher.Should().NotBeNull();
            sut.Publisher.Id.Should().Be(publisher.Id);
            sut.Authors.Count.Should().Be(2);
            sut.ReadDates.Count.Should().Be(2);
            sut.Formats.Count.Should().Be(2);
            sut.Genres.Count.Should().Be(2);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("A Book")]
        [InlineData("self-help")]
        [InlineData("Så är det")]
        [InlineData("Bacon ipsum dolor amet flank bresaola tongue, chislic shank swine beef buffalo ball tip rump. Prosciutto ground round meatloaf jowl sausage pig flank kielbasa landjaeger cow tail pork ball tip. Tail beef ham hock cupim salami meatball, drumstick chuck porc")]
        public void Valid_title(string title)
        {
            var sut = CreateBook();
            sut.SetTitle(title);
            sut.Title.Should().Be(title);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("Bacon ipsum dolor amet flank bresaola tongue, chislic shank swine beef buffalo ball tip rump. Prosciutto ground round meatloaf jowl sausage pig flank kielbasa landjaeger cow tail pork ball tip. Tail beef ham hock cupim salami meatball, drumstick chuck porch")]
        public void InValid_title(string title)
        {
            var sut = CreateBook();
            Action action = () => sut.SetTitle(title);

            action.Should().Throw<InvalidTitleException>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2_500)]
        public void Valid_ReleaseYear(int releaseYear)
        {
            var sut = CreateBook();
            sut.SetReleaseYear(releaseYear);
            sut.ReleaseYear.Should().Be(releaseYear);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2_501)]
        public void Invalid_ReleaseYear(int releaseYear)
        {
            var sut = CreateBook();
            Action action = () => sut.SetReleaseYear(releaseYear);

            action.Should().Throw<ArgumentException>();
        }


        [Theory]
        [InlineData(1)]
        [InlineData(99_999)]
        public void Valid_PageCount(int pageCount)
        {
            var sut = CreateBook();
            sut.SetPageCount(pageCount);
            sut.PageCount.Should().Be(pageCount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100_000)]
        public void Invalid_PageCount(int pageCount)
        {
            var sut = CreateBook();
            Action action = () => sut.SetPageCount(pageCount);

            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(1_000_000)]
        public void Valid_WordCount(int wordCount)
        {
            var sut = CreateBook();
            sut.SetWordCount(wordCount);
            sut.WordCount.Should().Be(wordCount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Invalid_WordCount(int wordCount)
        {
            var sut = CreateBook();
            Action action = () => sut.SetWordCount(wordCount);

            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("0553103547")]
        [InlineData("000224585X")]
        [InlineData("0553106635")]
        [InlineData("0002247437")]
        [InlineData("9780553801477")]
        [InlineData("9781566199094")]
        [InlineData("9781402894626")]
        [InlineData("")]
        public void Valid_Isbn(string isbn)
        {
            var sut = CreateBook();
            sut.SetIsbn(isbn);
            sut.Isbn.Should().Be(isbn);
        }

        [Theory]
        [InlineData("9780553801477Y")]
        [InlineData("000224585Z")]
        [InlineData("ABCDEFGHI")]
        [InlineData("978055380147XX")]
        [InlineData("000224585ZRR")]
        [InlineData("ABCDEFGHIJKLM")]
        public void Invalid_Isbn(string isbn)
        {
            var sut = CreateBook();
            Action action = () => sut.SetIsbn(isbn);

            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("fake.jpg")]
        [InlineData("fake.JPG")]
        [InlineData("fake.jpeg")]
        [InlineData("fake.png")]
        [InlineData("fake.gif")]
        public void Valid_cover_path(string file)
        {
            var sut = CreateBook();
            var pic = @$"C:\temp\testingsutPicsPath\{file}";
            sut.SetBookCoverPath(pic);

            sut.BookCoverPath.Should().Be(pic);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(@"C:\secret.doc%00.exe")]
        [InlineData(@"C:\fake.bmp")]
        [InlineData(@"C:\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long_path\is_too_long\too_long123\real.jpg")]
        public void Invalid_cover_path(string path)
        {
            var sut = CreateBook();
            var pic = path;
            Action action = () => sut.SetBookCoverPath(pic);

            action.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("")]
        public void Valid_Description(string description)
        {
            var sut = CreateBook();
            sut.SetDescription(description);

            sut.Description.Should().BeOfType<string>();
            sut.Description.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        public void Valid_Notes(string notes)
        {
            var sut = CreateBook();
            sut.SetNotes(notes);

            sut.Notes.Should().BeOfType<string>();
            sut.Notes.Should().BeEmpty();
        }

        [Fact]
        public void IsRead()
        {
            var sut = CreateBook();
            sut.IsRead.Should().BeFalse();
            sut.SetIsRead(true);
            sut.IsRead.Should().BeTrue();
        }

        [Fact]
        public void Valid_Language()
        {
            var sut = CreateBook();
            sut.Language.Should().BeNull();
            var language = Language.NewLanguage;
            sut.SetLanguage(language);

            sut.Language.Should().NotBeNull();
        }

        [Fact]
        public void Valid_Publisher()
        {
            var sut = CreateBook();
            sut.Publisher.Should().BeNull();
            var publisher = Publisher.NewPublisher;
            sut.SetPublisher(publisher);

            sut.Publisher.Should().NotBeNull();
        }

        [Fact]
        public void Valid_Authors()
        {
            var sut = CreateBook();
            sut.Authors.Count.Should().Be(0);

            var newAuthors = new List<Author>
            {
                Author.NewAuthor,
                Author.NewAuthor
            };

            sut.SetAuthors(newAuthors);

            sut.Authors.Should().NotBeNull();
            sut.Authors.Count.Should().Be(2);
        }

        [Fact]
        public void Valid_Genres()
        {
            var sut = CreateBook();
            sut.Genres.Count.Should().Be(0);

            var newGenres = new List<Genre>
            {
                Genre.NewGenre,
                Genre.NewGenre
            };

            sut.SetGenres(newGenres);

            sut.Genres.Should().NotBeNull();
            sut.Genres.Count.Should().Be(2);
        }

        [Fact]
        public void Valid_Formats()
        {
            var sut = CreateBook();
            sut.Formats.Count.Should().Be(0);

            var newFormats = new List<Format>
            {
                Format.NewFormat,
                Format.NewFormat
            };

            sut.SetFormats(newFormats);

            sut.Formats.Should().NotBeNull();
            sut.Formats.Count.Should().Be(2);
        }

        [Fact]
        public void Valid_ReadDates()
        {
            var sut = CreateBook();
            sut.ReadDates.Count.Should().Be(0);
            var newReadDates = new List<BookReadDate>
            {
                new BookReadDate(),
                new BookReadDate()
            };


            sut.SetReadDates(newReadDates);

            sut.ReadDates.Should().NotBeNull();
            sut.ReadDates.Count.Should().Be(2);
        }
    }
}
