﻿using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BookOrganizer2.Domain.AuthorProfile.NationalityProfile
{
    public class Nationality : IIdentifiable<NationalityId>
    {
        public NationalityId Id { get; private set; }
        public string Name { get; private set; }
        public ICollection<Author> Authors { get; set; }

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
            const string msg =
                "Invalid name. \nName should be 1-32 characters long.\nName may not contain non alphabet characters.";

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