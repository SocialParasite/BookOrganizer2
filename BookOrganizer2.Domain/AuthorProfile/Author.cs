using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.Common;
using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BookOrganizer2.Domain.AuthorProfile
{
    public sealed class Author : IIdentifiable<AuthorId>
    {
        private Author()
        {
            Notes ??= new ObservableCollection<Note>();
        }

        public AuthorId Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public string Biography { get; private set; }
        public string MugshotPath { get; private set; }
        public string NotesOld { get; private set; }
        public ICollection<Note> Notes { get; set; }
        public Nationality Nationality { get; private set; }
        public ICollection<Book> Books { get; set; }

        public static Author Create(AuthorId id,
                                    string firstName,
                                    string lastName,
                                    DateTime? dob = null,
                                    string biography = null,
                                    string mugshotPath = null,
                                    string notesOld = null,
                                    ICollection<Note> notes = null,
                                    Nationality nationality = null)
        {
            ValidateParameters();

            var author = new Author();

            author.Apply(new Events.AuthorCreated
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dob,
                Biography = biography,
                MugshotPath = mugshotPath,
                NotesOld = notesOld,
                Nationality = nationality,
                Notes = notes
            });

            return author;

            void ValidateParameters()
            {
                if (id is null)
                    throw new ArgumentNullException(nameof(id), "Author without unique identifier cannot be created.");
                if (string.IsNullOrWhiteSpace(firstName))
                    throw new ArgumentNullException(nameof(firstName), "Author without first name cannot be created.");
                if (string.IsNullOrWhiteSpace(lastName))
                    throw new ArgumentNullException(nameof(firstName), "Author without last name cannot be created.");
            }
        }

        public static Author NewAuthor => new() { Id = new AuthorId(SequentialGuid.NewSequentialGuid()) };

        public void SetFirstName(string name)
        {
            const string msg = "Invalid first name. \nName should be 1-64 characters long.\nName may not contain non alphabet characters.";
            if (ValidateName(name))
                Apply(new Events.AuthorsFirstNameChanged
                {
                    Id = Id,
                    FirstName = name
                });
            else
                throw new InvalidFirstNameException(msg);
        }

        public void SetLastName(string name)
        {
            const string msg = "Invalid last name. \nName should be 1-64 characters long.\nName may not contain non alphabet characters.";
            if (ValidateName(name))
                Apply(new Events.AuthorsLastNameChanged
                {
                    Id = Id,
                    LastName = name
                });
            else
                throw new InvalidLastNameException(msg);
        }

        public void SetDateOfBirth(DateTime? dob)
        {
            Apply(new Events.AuthorDateOfBirthChanged
            {
                Id = Id,
                DateOfBirth = dob
            });
        }

        public void SetBiography(string bio)
        {
            Apply(new Events.AuthorsBiographyChanged
            {
                Id = Id,
                Biography = bio
            });
        }

        public void SetNotesOld(string notes)
        {
            Apply(new Events.AuthorsNotesOldChanged
            {
                Id = Id,
                NotesOld = notes
            });
        }

        public void SetNotes(ICollection<Note> notes)
        {
            Apply(new Events.AuthorsNotesChanged
            {
                Id = Id,
                Notes = notes
            });
        }

        public void SetMugshotPath(string pic)
        {
            if (pic.Length > 256)
                throw new ArgumentException();

            var path = Path.GetFullPath(pic);
            string[] formats = { ".jpg", ".png", ".gif", ".jpeg" };

            if (formats.Contains(Path.GetExtension(pic), StringComparer.InvariantCultureIgnoreCase))
                Apply(new Events.AuthorsMugshotPathChanged
                {
                    Id = Id,
                    MugshotPath = path
                });
            else
                throw new Exception();
        }

        public void SetNationality(Nationality nationality)
        {
            Apply(new Events.NationalityChanged
            {
                Id = Id,
                Nationality = nationality
            });
        }

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
                   && !string.IsNullOrWhiteSpace(FirstName)
                   && !string.IsNullOrWhiteSpace(LastName);
        }

        private void Apply(object @event)
        {
            When(@event);
        }

        private void When(object @event)
        {
            switch (@event)
            {
                case Events.AuthorCreated e:
                    Id = new AuthorId(e.Id);
                    FirstName = e.FirstName;
                    LastName = e.LastName;
                    DateOfBirth = e.DateOfBirth;
                    MugshotPath = e.MugshotPath;
                    Biography = e.Biography;
                    NotesOld = e.NotesOld;
                    Nationality = e.Nationality;
                    Notes = e.Notes;
                    break;
                case Events.AuthorDateOfBirthChanged e:
                    DateOfBirth = e.DateOfBirth;
                    break;
                case Events.AuthorsFirstNameChanged e:
                    Id = e.Id;
                    FirstName = e.FirstName;
                    break;
                case Events.AuthorsLastNameChanged e:
                    Id = e.Id;
                    LastName = e.LastName;
                    break;
                case Events.AuthorsBiographyChanged e:
                    Id = e.Id;
                    Biography = e.Biography;
                    break;
                case Events.AuthorsMugshotPathChanged e:
                    Id = e.Id;
                    MugshotPath = e.MugshotPath;
                    break;
                case Events.AuthorsNotesOldChanged e:
                    Id = e.Id;
                    NotesOld = e.NotesOld;
                    break;
                case Events.AuthorsNotesChanged e:
                    Id = e.Id;
                    (Notes as List<Note>)?.AddRange(e.Notes);
                    break;
                case Events.NationalityChanged e:
                    Id = e.Id;
                    Nationality = e.Nationality;
                    break;
                case Events.AuthorDeleted e:
                    Id = e.Id;
                    break;
            }
        }
    }
}
