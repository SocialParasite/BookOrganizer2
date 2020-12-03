using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.BookProfile.SeriesProfile
{
    public class Series : IIdentifiable<SeriesId>
    {
        public SeriesId Id { get; private set; }
        public string Name { get; private set; }
        //public int NumberOfBooks { get; private set; }
        public string PicturePath { get; private set; }
        public string Description { get; private set; }
        public ICollection<Book> Books { get; set; }

        // series read order => instalment! EF many-to-many 

        public static Series Create(SeriesId id, string name, string picturePath = null, string description = null)
        {
            ValidateParameters();

            var series = new Series();

            series.Apply(new Events.SeriesCreated
            {
                Id = id,
                Name = name,
                PicturePath = picturePath,
                Description = description
            });

            return series;

            void ValidateParameters()
            {
                if (id is null)
                    throw new ArgumentNullException(nameof(id),
                        "Series without unique identifier cannot be created.");
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name), "Series without name cannot be created.");
            }
        }

        public static Series NewSeries
            => new Series { Id = new SeriesId(SequentialGuid.NewSequentialGuid()) };

        public void SetName(string name)
        {
            const string msg =
                "Invalid name. \nName should be 1-32 characters long.\nName may not contain non alphabet characters.";
            if (ValidateName(name))
                Name = name;
            else
                throw new InvalidNameException(msg);
        }

        public void SetPicturePath(string pic)
        {
            if (pic.Length > 256)
                throw new ArgumentException();

            var path = Path.GetFullPath(pic);
            string[] formats = { ".jpg", ".png", ".gif", ".jpeg" };

            if (formats.Contains(Path.GetExtension(pic), StringComparer.InvariantCultureIgnoreCase))
                Apply(new Events.SeriesPicturePathChanged
                {
                    Id = Id,
                    PicturePath = path
                });
            else
                throw new Exception();
        }

        public void SetDescription(string description)
        {
            Apply(new Events.SeriesDescriptionChanged
            {
                Id = Id,
                Description = description
            });
        }

        internal bool EnsureValidState()
        {
            return Id.Value != default
                   && !string.IsNullOrWhiteSpace(Name);
        }

        private static bool ValidateName(string name)
        {
            const int minLength = 1;
            const int maxLength = 256;
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
                case Events.SeriesCreated e:
                    Id = new SeriesId(e.Id);
                    Name = e.Name;
                    PicturePath = e.PicturePath;
                    Description = e.Description;
                    break;
                case Events.SeriesUpdated e:
                    Name = e.Name;
                    PicturePath = e.PicturePath;
                    Description = e.Description;
                    break;
                case Events.SeriesPicturePathChanged e:
                    Id = e.Id;
                    PicturePath = e.PicturePath;
                    break;
                case Events.SeriesDescriptionChanged e:
                    Id = e.Id;
                    Description = e.Description;
                    break;
                case Events.SeriesDeleted e:
                    Id = e.Id;
                    break;
            }
        }
    }
}