using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BookOrganizer2.Domain.Helpers.Extensions;

namespace BookOrganizer2.Domain.AuthorProfile.NationalityProfile
{
    public class Nationality : IIdentifiable<NationalityId>
    {
        public NationalityId Id { get; private set; }
        public string Name { get; private set; }
        public ICollection<Author> Authors { get; set; }
        private const int MinLength = 1;
        private const int MaxLength = 32;

        public static Nationality NewNationality
            => new() { Id = new NationalityId(SequentialGuid.NewSequentialGuid()) };

        public static Nationality Create(NationalityId id, string name)
        {
            ValidateParameters();

            var nationality = new Nationality();

            nationality.Apply(new Events.Created
            {
                Id = id,
                Name = name
            });

            return nationality;

            void ValidateParameters()
            {
                if (id is null)
                {
                    throw new ArgumentNullException(nameof(id),
                        "Nationality without unique identifier cannot be created.");
                }

                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name), "Nationality without name cannot be created.");
            }
        }

        public void SetName(string name)
        {
            var msg = $"Invalid name. \nName should be {MinLength}-{MaxLength} characters long.\nName may not contain non alphabet characters.";

            if (ValidateName(name))
            {
                Apply(new Events.Updated
                {
                    Id = Id,
                    Name = name
                });
            }
            else
            {
                throw new InvalidNameException(msg);
            }
        }

        internal bool EnsureValidState() 
            => Id.Value.HasNonDefaultValue() && !string.IsNullOrWhiteSpace(Name);

        private static bool ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var pattern = "(?=.{" + MinLength + "," + MaxLength + "}$)^[\\p{L}\\p{M}\\s'-]+?$";

            var regexPattern = new Regex(pattern);

            return regexPattern.IsMatch(name);
        }

        private void Apply(object @event) => When(@event);

        private void When(object @event)
        {
            switch (@event)
            {
                case Events.Created e:
                    Id = new NationalityId(e.Id);
                    Name = e.Name;
                    break;
                case Events.Updated e:
                    Name = e.Name;
                    break;
                case Events.Deleted e:
                    Id = e.Id;
                    break;
            }
        }
    }
}