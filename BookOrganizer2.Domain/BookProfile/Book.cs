using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.BookProfile
{
    public class Book : IIdentifiable<BookId>
    {
        private Book() { }

        public BookId Id { get; private set; }
        public string Title { get; private set; }
        public int ReleaseYear { get; private set; }
        public int PageCount { get; private set; }
        public int WordCount { get; private set; }
        public string Isbn { get; private set; }
        public string BookCoverPath { get; private set; }
        public string Description { get; private set; }
        public string Notes { get; private set; }
        public bool IsRead { get; private set; }
        public Language Language { get; private set; }
        public Publisher Publisher { get; private set; }
        public ICollection<Author> Authors { get; set; }
        public ICollection<BookReadDate> ReadDates { get; set; }
        public ICollection<Format> Formats { get; set; }
        public ICollection<Genre> Genres { get; set; }

        //public static Book Create(BookId id,
        //                            string title,
        //                            string lastName,
        //                            DateTime? dob = null,
        //                            string biography = null,
        //                            string mugshotPath = null,
        //                            string notes = null,
        //                            Nationality nationality = null)
        //{
        //    ValidateParameters();

        //    var book = new Book();

        //    book.Apply(new Events.BookCreated
        //    {
        //        Id = id,
        //        Title = title,
        //        LastName = lastName,
        //        DateOfBirth = dob,
        //        Biography = biography,
        //        MugshotPath = mugshotPath,
        //        Notes = notes,
        //        Nationality = nationality
        //    });

        //    return book;

        //    void ValidateParameters()
        //    {
        //        if (id is null)
        //            throw new ArgumentNullException(nameof(id), "Book without unique identifier cannot be created.");
        //        if (string.IsNullOrWhiteSpace(title))
        //            throw new ArgumentNullException(nameof(title), "Book without first name cannot be created.");
        //        if (string.IsNullOrWhiteSpace(lastName))
        //            throw new ArgumentNullException(nameof(title), "Book without last name cannot be created.");
        //    }
        //}

        public static Book NewBook => new Book { Id = new BookId(SequentialGuid.NewSequentialGuid()) };

        //public void SetTitle(string name)
        //{
        //    const string msg = "Invalid first name. \nName should be 1-64 characters long.\nName may not contain non alphabet characters.";
        //    if (ValidateName(name))
        //        Apply(new Events.BooksTitleChanged
        //        {
        //            Id = Id,
        //            Title = name
        //        });
        //    else
        //        throw new InvalidTitleException(msg);
        //}

        //public void SetLastName(string name)
        //{
        //    const string msg = "Invalid last name. \nName should be 1-64 characters long.\nName may not contain non alphabet characters.";
        //    if (ValidateName(name))
        //        Apply(new Events.BooksLastNameChanged
        //        {
        //            Id = Id,
        //            LastName = name
        //        });
        //    else
        //        throw new InvalidLastNameException(msg);
        //}

        //public void SetDateOfBirth(DateTime? dob)
        //{
        //    Apply(new Events.BookDateOfBirthChanged
        //    {
        //        Id = Id,
        //        DateOfBirth = dob
        //    });
        //}

        //public void SetBiography(string bio)
        //{
        //    Apply(new Events.BooksBiographyChanged
        //    {
        //        Id = Id,
        //        Biography = bio
        //    });
        //}

        //public void SetNotes(string notes)
        //{
        //    Apply(new Events.BooksNotesChanged
        //    {
        //        Id = Id,
        //        Notes = notes
        //    });
        //}

        //public void SetMugshotPath(string pic)
        //{
        //    if (pic.Length > 256)
        //        throw new ArgumentException();

        //    var path = Path.GetFullPath(pic);
        //    string[] formats = { ".jpg", ".png", ".gif", ".jpeg" };

        //    if (formats.Contains(Path.GetExtension(pic), StringComparer.InvariantCultureIgnoreCase))
        //        Apply(new Events.BooksMugshotPathChanged
        //        {
        //            Id = Id,
        //            MugshotPath = path
        //        });
        //    else
        //        throw new Exception();
        //}

        //public void SetNationality(Nationality nationality)
        //{
        //    Apply(new Events.NationalityChanged
        //    {
        //        Id = Id,
        //        Nationality = nationality
        //    });
        //}

        private static bool ValidateName(string name)
        {
            const int minLength = 1;
            const int maxLength = 64;
            var pattern = "(?=.{" + minLength + "," + maxLength + "}$)^[\\p{L}\\p{M}\\s'-]+?$";

            if (string.IsNullOrWhiteSpace(name))
                return false;

            var regexPattern = new Regex(pattern);

            return regexPattern.IsMatch(name);
        }

        internal bool EnsureValidState()
        {
            return Id.Value != default
                   && !string.IsNullOrWhiteSpace(Title);
        }

        //private void Apply(object @event)
        //{
        //    When(@event);
        //}

        //private void When(object @event)
        //{
        //    switch (@event)
        //    {
        //        case Events.BookCreated e:
        //            Id = new BookId(e.Id);
        //            Title = e.Title;
        //            LastName = e.LastName;
        //            DateOfBirth = e.DateOfBirth;
        //            MugshotPath = e.MugshotPath;
        //            Biography = e.Biography;
        //            Notes = e.Notes;
        //            Nationality = e.Nationality;
        //            break;
        //        case Events.BookDateOfBirthChanged e:
        //            DateOfBirth = e.DateOfBirth;
        //            break;
        //        case Events.BooksTitleChanged e:
        //            Id = e.Id;
        //            Title = e.Title;
        //            break;
        //        case Events.BooksLastNameChanged e:
        //            Id = e.Id;
        //            LastName = e.LastName;
        //            break;
        //        case Events.BooksBiographyChanged e:
        //            Id = e.Id;
        //            Biography = e.Biography;
        //            break;
        //        case Events.BooksMugshotPathChanged e:
        //            Id = e.Id;
        //            MugshotPath = e.MugshotPath;
        //            break;
        //        case Events.BooksNotesChanged e:
        //            Id = e.Id;
        //            Notes = e.Notes;
        //            break;
        //        case Events.NationalityChanged e:
        //            Id = e.Id;
        //            Nationality = e.Nationality;
        //            break;
        //        case Events.BookDeleted e:
        //            Id = e.Id;
        //            break;
        //    }
        //}
    }
}
