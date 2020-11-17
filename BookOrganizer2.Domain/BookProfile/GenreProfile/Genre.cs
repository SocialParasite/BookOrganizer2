using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BookOrganizer2.Domain.BookProfile.GenreProfile
{
    public class Genre : IIdentifiable<GenreId>
    {
        public GenreId Id { get; private set; }
        public string Name { get; private set; }
        public ICollection<Book> Books { get; set; }
        public static Genre Create(GenreId id, string name)
        {
            ValidateParameters();

            var genre = new Genre();

            genre.Apply(new Events.GenreCreated
            {
                Id = id,
                Name = name
            });

            return genre;

            void ValidateParameters()
            {
                if (id is null)
                    throw new ArgumentNullException(nameof(id),
                        "Genre without unique identifier cannot be created.");
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name), "Genre without name cannot be created.");
            }
        }

        public static Genre NewGenre
            => new Genre { Id = new GenreId(SequentialGuid.NewSequentialGuid()) };

        public void SetName(string name)
        {
            const string msg =
                "Invalid name. \nName should be 1-32 characters long.\nName may not contain non alphabet characters.";
            if (ValidateName(name))
                Name = name;
            else
                throw new InvalidNameException(msg);
        }

        internal bool EnsureValidState()
        {
            return Id.Value != default
                   && !string.IsNullOrWhiteSpace(Name);
        }

        private static bool ValidateName(string name)
        {
            const int minLength = 1;
            const int maxLength = 32;
            var pattern = "(?=.{" + minLength + "," + maxLength + "}$)^[\\p{L}\\p{M}\\s'-]+?$";

            if (string.IsNullOrWhiteSpace(name))
                return false;

            var regexPattern = new Regex(pattern);

            return regexPattern.IsMatch(name);
        }

        private void Apply(object @event)
        {
            When(@event);
        }

        private void When(object @event)
        {
            switch (@event)
            {
                case Events.GenreCreated e:
                    Id = new GenreId(e.Id);
                    Name = e.Name;
                    break;
                case Events.GenreUpdated e:
                    Name = e.Name;
                    break;
                case Events.GenreDeleted e:
                    Id = e.Id;
                    break;
            }
        }
    }
}