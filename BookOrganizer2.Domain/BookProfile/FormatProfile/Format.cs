﻿using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BookOrganizer2.Domain.BookProfile.FormatProfile
{
    public class Format : IIdentifiable<FormatId>
    {
        public FormatId Id { get; private set; }
        public string Name { get; private set; }
        public ICollection<Book> Books { get; set; }
        private const int MinLength = 1;
        private const int MaxLength = 32;

        public static Format Create(FormatId id, string name)
        {
            ValidateParameters();

            var format = new Format();

            format.Apply(new Events.Created
            {
                Id = id,
                Name = name
            });

            return format;

            void ValidateParameters()
            {
                if (id is null)
                {
                    throw new ArgumentNullException(nameof(id),
                        "Format without unique identifier cannot be created.");
                }

                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name), "Format without name cannot be created.");
            }
        }

        public static Format NewFormat
            => new() { Id = new FormatId(SequentialGuid.NewSequentialGuid()) };

        public void SetName(string name)
        {
            var msg = $"Invalid name. \nName should be {MinLength}-{MaxLength} characters long.\nName may not contain non alphabet characters.";
            if (ValidateName(name))
                Name = name;
            else
                throw new InvalidNameException(msg);
        }
        
        internal bool EnsureValidState()
        {
            return HasNonDefaultId() && !string.IsNullOrWhiteSpace(Name);

            bool HasNonDefaultId() => Id.Value != default;
        }

        private static bool ValidateName(string name)
        {
            var pattern = "(?=.{" + MinLength + "," + MaxLength + "}$)^[\\p{L}\\p{M}\\s'-]+?$";

            if (string.IsNullOrWhiteSpace(name))
                return false;

            var regexPattern = new Regex(pattern);

            return regexPattern.IsMatch(name);
        }

        private void Apply(object @event) => When(@event);

        private void When(object @event)
        {
            switch (@event)
            {
                case Events.Created e:
                    Id = new FormatId(e.Id);
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