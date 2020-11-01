using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.Shared;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BookOrganizer2.Domain.AuthorProfile
{
    public class Author : IIdentifiable<AuthorId>
    {
        private Author() { }

        public AuthorId Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public string Biography { get; private set; }
        public string MugshotPath { get; private set; }
        public string Notes { get; private set; }

        public static Author Create(AuthorId id, 
                                    string firstName, 
                                    string lastName, 
                                    DateTime? dob = null, 
                                    string biography = null, 
                                    string mugshotPath = null, 
                                    string notes = null)
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

        public static Author NewAuthor => new Author();

        public void SetFirstName(string name)
        {
            if(ValidateName(name, () => throw new InvalidFirstNameException()))
                FirstName = name;
            else 
                throw new InvalidFirstNameException();
        }

        public void SetLastName(string name)
        {
            if (ValidateName(name, () => throw new InvalidLastNameException(null)))
                LastName = name;
            else
                throw new InvalidLastNameException("Invalid last name. Name may not contain non alphabet characters.");
        }

        public void SetDateOfBirth(DateTime? dob)
        {
            DateOfBirth = dob;
        }

        public void SetBiography(string bio)
        {
            Biography = bio;
        }

        public void SetNotes(string notes)
        {
            Notes = notes;
        }

        public void SetMugshotPath(string pic)
        {
            var path = Path.GetFullPath(pic);
            string[] formats = { ".jpg", ".png", ".gif", ".jpeg" };

            if (formats.Contains(Path.GetExtension(pic), StringComparer.InvariantCultureIgnoreCase))
                MugshotPath = path;
            else 
                throw new Exception();
        }

        private static bool ValidateName(string name, Action exception)
        {
            const int minLength = 1;
            const int maxLength = 64;
            var pattern = "(?=.{" + minLength + "," + maxLength +"}$)^[\\p{L}\\p{M}\\s'-]+?$";

            if (string.IsNullOrWhiteSpace(name)) 
                exception.Invoke();

            var regexPattern = new Regex(pattern);

            return regexPattern.IsMatch(name ?? string.Empty);   
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
                case Events.AuthorsNotesChanged e:
                    Id = e.Id;
                    Notes = e.Notes;
                    break;
                case Events.AuthorDeleted e:
                    Id = e.Id;
                    break;
            }
        }
    }
}
