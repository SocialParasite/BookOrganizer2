using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using BookOrganizer2.Domain.BookProfile;

namespace BookOrganizer2.Domain.PublisherProfile
{
    public class Publisher : IIdentifiable<PublisherId>
    {
        private Publisher() { }

        public PublisherId Id { get; private set; }
        public string Name { get; private set; }
        public string LogoPath { get; private set; }
        public string Description { get; private set; }
        public ICollection<Book> Books { get; set; }



        public static Publisher Create(PublisherId id,
                                    string name,
                                    string logoPath = null,
                                    string description = null)
        {
            ValidateParameters();

            var publisher = new Publisher();

            publisher.Apply(new Events.PublisherCreated
            {
                Id = id,
                Name = name,
                LogoPath = logoPath,
                Description = description
            });

            return publisher;

            void ValidateParameters()
            {
                if (id is null)
                    throw new ArgumentNullException(nameof(id), "Publisher without unique identifier cannot be created.");
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name), "Publisher without name cannot be created.");
            }
        }

        public static Publisher NewPublisher => new Publisher { Id = new PublisherId(SequentialGuid.NewSequentialGuid()) };

        public void SetName(string name)
        {
            const string msg = "Invalid name. \nName should be 1-64 characters long.\nName may not contain non alphabet characters.";
            if (ValidateName(name))
                Apply(new Events.PublishersNameChanged
                {
                    Id = Id,
                    Name = name
                });
            else
                throw new InvalidFirstNameException(msg);
        }

        public void SetDescription(string desc)
        {
            Apply(new Events.PublishersDescriptionChanged
            {
                Id = Id,
                Description = desc
            });
        }

        public void SetLogoPath(string pic)
        {
            if (pic.Length > 256)
                throw new ArgumentException();

            var path = Path.GetFullPath(pic);
            string[] formats = { ".jpg", ".png", ".gif", ".jpeg" };

            if (formats.Contains(Path.GetExtension(pic), StringComparer.InvariantCultureIgnoreCase))
                Apply(new Events.PublishersLogoPathChanged
                {
                    Id = Id,
                    LogoPath = path
                });
            else
                throw new Exception();
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
                   && !string.IsNullOrWhiteSpace(Name);
        }

        private void Apply(object @event)
        {
            When(@event);
        }

        private void When(object @event)
        {
            switch (@event)
            {
                case Events.PublisherCreated e:
                    Id = new PublisherId(e.Id);
                    Name = e.Name;
                    LogoPath = e.LogoPath;
                    Description = e.Description;
                    break;
                case Events.PublishersNameChanged e:
                    Id = e.Id;
                    Name = e.Name;
                    break;
                case Events.PublishersDescriptionChanged e:
                    Id = e.Id;
                    Description = e.Description;
                    break;
                case Events.PublishersLogoPathChanged e:
                    Id = e.Id;
                    LogoPath = e.LogoPath;
                    break;
                case Events.PublisherDeleted e:
                    Id = e.Id;
                    break;
            }
        }
    }
}
