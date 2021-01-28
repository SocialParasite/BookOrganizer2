using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.Shared;
using System;
using System.Text.RegularExpressions;

namespace BookOrganizer2.Domain.BookProfile.LanguageProfile
{
    public class Language : IIdentifiable<LanguageId>
    {
        public LanguageId Id { get; private set; }
        public string Name { get; private set; }

        public static Language Create(LanguageId id, string name)
        {
            ValidateParameters();

            var language = new Language();

            language.Apply(new Events.LanguageCreated
            {
                Id = id,
                Name = name
            });

            return language;

            void ValidateParameters()
            {
                if (id is null)
                {
                    throw new ArgumentNullException(nameof(id),
                        "Language without unique identifier cannot be created.");
                }

                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name), "Language without name cannot be created.");
            }
        }

        public static Language NewLanguage
            => new() { Id = new LanguageId(SequentialGuid.NewSequentialGuid()) };

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
                case Events.LanguageCreated e:
                    Id = new LanguageId(e.Id);
                    Name = e.Name;
                    break;
                case Events.LanguageUpdated e:
                    Name = e.Name;
                    break;
                case Events.LanguageDeleted e:
                    Id = e.Id;
                    break;
            }
        }
    }
}