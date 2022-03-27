using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BookOrganizer2.Domain.Common;

namespace BookOrganizer2.Domain.BookProfile
{
    public sealed class Book : IIdentifiable<BookId>
    {
        private Book()
        {
            Authors ??= new ObservableCollection<Author>();
            Formats ??= new ObservableCollection<Format>();
            Genres ??= new ObservableCollection<Genre>();
            ReadDates ??= new ObservableCollection<BookReadDate>();
            Notes ??= new ObservableCollection<Note>();
        }

        public BookId Id { get; private set; }
        public string Title { get; private set; }
        public int ReleaseYear { get; private set; }
        public int PageCount { get; private set; }
        public int WordCount { get; private set; }
        public string Isbn { get; private set; }
        public string BookCoverPath { get; private set; }
        public string Description { get; private set; }
        public string NotesOld { get; private set; }
        public ICollection<Note> Notes { get; set; }
        //public ICollection<Attachment> Attachments { get; set; } // maps, etc.
        public bool IsRead { get; private set; }
        public Language Language { get; private set; }
        public Publisher Publisher { get; private set; }
        public ICollection<Author> Authors { get; private set; }
        public ICollection<BookReadDate> ReadDates { get; private set; }
        public ICollection<Format> Formats { get; private set; }
        public ICollection<Genre> Genres { get; private set; }
        public ICollection<ReadOrder> Series { get; private set; }

        public static Book Create(BookId id,
                                    string title,
                                    int releaseYear = 0,
                                    int pageCount = 0,
                                    int wordCount = 0,
                                    string isbn = null,
                                    string bookCoverPath = null,
                                    string description = null,
                                    string notes = null,
                                    bool isRead = false,
                                    Language language = null,
                                    Publisher publisher = null,
                                    ICollection<Author> authors = null,
                                    ICollection<BookReadDate> bookReadDates = null,
                                    ICollection<Format> formats = null,
                                    ICollection<Genre> genres = null)
        {
            ValidateParameters();

            var book = new Book();

            book.Apply(new Events.Created
            {
                Id = id,
                Title = title,
                ReleaseYear = releaseYear,
                PageCount = pageCount,
                WordCount = wordCount,
                Isbn = isbn,
                BookCoverPath = bookCoverPath,
                Description = description,
                Notes = notes,
                IsRead = isRead,
                Language = language,
                Publisher = publisher,
                Authors = authors, 
                BookReadDates = bookReadDates, 
                Formats = formats, 
                Genres = genres
            });

            return book;

            void ValidateParameters()
            {
                if (id is null)
                    throw new ArgumentNullException(nameof(id), "Book without unique identifier cannot be created.");
                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentNullException(nameof(title), "Book without title cannot be created.");
            }
        }

        public static Book NewBook => new() { Id = new BookId(SequentialGuid.NewSequentialGuid()) };

        public void SetTitle(string name)
        {
            const string msg = "Invalid first name. \nName should be 1-256 characters long.";
            if (ValidateName(name))
            {
                Apply(new Events.TitleChanged
                {
                    Id = Id,
                    Title = name
                });
            }
            else
            {
                throw new InvalidTitleException(msg);
            }

            static bool ValidateName(string title)
            {
                if (string.IsNullOrWhiteSpace(title))
                    return false;

                return title.Length is >= 1 and <= 256;
            }
        }

        public void SetReleaseYear(int releaseYear)
        {
            const string msg = "Invalid year. \nRelease year should be between 1 and 2500.";
            if (releaseYear is > 0 and < 2_501)
            {
                Apply(new Events.ReleaseYearChanged
                {
                    Id = Id,
                    ReleaseYear = releaseYear
                });
            }
            else
            {
                throw new ArgumentException(msg);
            }
        }

        public void SetPageCount(int pageCount)
        {
            const string msg = "Release year should be between 1 and 99 999.";
            if (pageCount is > 0 and < 100_000)
            {
                Apply(new Events.PageCountChanged
                {
                    Id = Id,
                    PageCount = pageCount
                });
            }
            else
            {
                throw new ArgumentException(msg);
            }
        }

        public void SetWordCount(int wordCount)
        {
            const string msg = "Word count should be more than 1.";
            if (wordCount > 1)
            {
                Apply(new Events.WordCountChanged
                {
                    Id = Id,
                    WordCount = wordCount
                });
            }
            else
            {
                throw new ArgumentException(msg);
            }
        }

        public void SetIsbn(string isbn)
        {
            if(!ValidateIsbn()) throw new ArgumentException();

            Apply(new Events.IsbnChanged
            {
                Id = Id,
                Isbn = isbn
            });

            bool ValidateIsbn()
            {
                if (isbn is null || isbn is "") return true;

                const string pattern = @"^(97(8|9))?\d{9}(\d|X)$";

                var rgx = new Regex(pattern, RegexOptions.IgnoreCase);

                return rgx.IsMatch(isbn);
            }
        }

        public void SetBookCoverPath(string pic)
        {
            if (pic.Length > 256)
                throw new ArgumentException();

            var path = Path.GetFullPath(pic);
            string[] formats = { ".jpg", ".png", ".gif", ".jpeg" };

            if (formats.Contains(Path.GetExtension(pic), StringComparer.InvariantCultureIgnoreCase))
            {
                Apply(new Events.BookCoverPathChanged
                {
                    Id = Id,
                    BookCoverPath = path
                });
            }
            else
            {
                throw new Exception();
            }
        }

        public void SetDescription(string description)
        {
            Apply(new Events.DescriptionChanged
            {
                Id = Id,
                Description = description
            });
        }

        public void SetNotesOld(string notes)
        {
            Apply(new Events.NotesOldChanged
            {
                Id = Id,
                NotesOld = notes
            });
        }

        public void SetNotes(ICollection<Note> notes)
        {
            Apply(new Events.BooksNotesChanged
            {
                Id = Id,
                Notes = notes
            });
        }

        public void SetIsRead(bool isRead)
        {
            Apply(new Events.IsReadChanged
            {
                Id = Id,
                IsRead = isRead
            });
        }

        public void SetLanguage(Language language)
        {
            Apply(new Events.LanguageChanged
            {
                Id = Id,
                Language = language
            });
        }

        public void SetPublisher(Publisher publisher)
        {
            Apply(new Events.PublisherChanged
            {
                Id = Id,
                Publisher = publisher
            });
        }

        public void SetAuthors(ICollection<Author> authors)
        {
            Apply(new Events.AuthorsChanged
            {
                Id = Id,
                Authors = authors
            });
        }

        public void SetGenres(ICollection<Genre> genres)
        {
            Apply(new Events.GenresChanged
            {
                Id = Id,
                Genres = genres
            });
        }

        public void SetFormats(ICollection<Format> formats)
        {
            Apply(new Events.FormatsChanged
            {
                Id = Id,
                Formats = formats
            });
        }

        public void SetReadDates(ICollection<BookReadDate> readDates)
        {
            Apply(new Events.BookReadDatesChanged
            {
                Id = Id,
                BookReadDates = readDates
            });
        }

        internal bool EnsureValidState() =>
            Id.Value != default && !string.IsNullOrWhiteSpace(Title);

        private void Apply(object @event)
        {
            When(@event);
        }

        private void When(object @event)
        {
            switch (@event)
            {
                case Events.Created e:
                    Id = new BookId(e.Id);
                    Title = e.Title;
                    ReleaseYear = e.ReleaseYear;
                    PageCount = e.PageCount;
                    WordCount = e.WordCount;
                    Isbn = e.Isbn;
                    BookCoverPath = e.BookCoverPath;
                    Description = e.Description;
                    NotesOld = e.Notes;
                    IsRead = e.IsRead;
                    Language = e.Language;
                    Publisher = e.Publisher;
                    Authors = e.Authors;
                    ReadDates = e.BookReadDates;
                    Formats = e.Formats;
                    Genres = e.Genres;
                    break;
                case Events.TitleChanged e:
                    Id = e.Id;
                    Title = e.Title;
                    break;
                case Events.ReleaseYearChanged e:
                    Id = e.Id;
                    ReleaseYear = e.ReleaseYear;
                    break;
                case Events.PageCountChanged e:
                    Id = e.Id;
                    PageCount = e.PageCount;
                    break;
                case Events.WordCountChanged e:
                    Id = e.Id;
                    WordCount = e.WordCount;
                    break;
                case Events.IsbnChanged e:
                    Id = e.Id;
                    Isbn = e.Isbn;
                    break;
                case Events.BookCoverPathChanged e:
                    Id = e.Id;
                    BookCoverPath = e.BookCoverPath;
                    break;
                case Events.DescriptionChanged e:
                    Id = e.Id;
                    Description = e.Description;
                    break;
                case Events.NotesOldChanged e:
                    Id = e.Id;
                    NotesOld = e.NotesOld;
                    break;
                case Events.BooksNotesChanged e:
                    Id = e.Id;
                    (Notes as List<Note>)?.AddRange(e.Notes);
                    break;
                case Events.IsReadChanged e:
                    Id = e.Id;
                    IsRead = e.IsRead;
                    break;
                case Events.LanguageChanged e:
                    Id = e.Id;
                    Language = e.Language;
                    break;
                case Events.PublisherChanged e:
                    Id = e.Id;
                    Publisher = e.Publisher;
                    break;
                case Events.AuthorsChanged e:
                    Id = e.Id;
                    Authors = e.Authors;
                    break;
                case Events.BookReadDatesChanged e:
                    Id = e.Id;
                    ReadDates = e.BookReadDates;
                    break;
                case Events.FormatsChanged e:
                    Id = e.Id;
                    Formats = e.Formats;
                    break;
                case Events.GenresChanged e:
                    Id = e.Id;
                    Genres = e.Genres;
                    break;
                case Events.Deleted e:
                    Id = e.Id;
                    break;
            }
        }
    }
}
