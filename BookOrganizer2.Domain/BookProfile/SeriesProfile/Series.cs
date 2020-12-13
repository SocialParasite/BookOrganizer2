using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookOrganizer2.Domain.Exceptions;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.BookProfile.SeriesProfile
{
    public class Series : IIdentifiable<SeriesId>
    {
        public SeriesId Id { get; private set; }
        public string Name { get; private set; }
        public string PicturePath { get; private set; }
        public string Description { get; private set; }
        public ICollection<ReadOrder> Books { get; set; }

        public static Series Create(SeriesId id, 
            string name, 
            string picturePath = null, 
            string description = null, 
            ICollection<ReadOrder> books = null)
        {
            ValidateParameters();

            var series = new Series();

            series.Apply(new Events.SeriesCreated
            {
                Id = id,
                Name = name,
                PicturePath = picturePath ?? "",
                Description = description ?? "",
                Books = books ?? new List<ReadOrder>()
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
                "Invalid name. \nName should be 1-256 characters long.\nName may not contain non alphabet characters.";
            if (ValidateName(name))
                Name = name;
            else
                throw new InvalidNameException(msg);

            static bool ValidateName(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    return false;

                return name.Length >= 1 && name.Length <= 256;
            }
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

        public void SetBooks(ICollection<ReadOrder> books)
        {
            Apply(new Events.BooksChanged
            {
                Id = Id,
                Books = books
            });
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
                case Events.SeriesCreated e:
                    Id = new SeriesId(e.Id);
                    Name = e.Name;
                    PicturePath = e.PicturePath;
                    Description = e.Description;
                    Books = e.Books;
                    break;
                case Events.SeriesUpdated e:
                    Name = e.Name;
                    PicturePath = e.PicturePath;
                    Description = e.Description;
                    Books = e.Books;
                    break;
                case Events.SeriesPicturePathChanged e:
                    Id = e.Id;
                    PicturePath = e.PicturePath;
                    break;
                case Events.SeriesDescriptionChanged e:
                    Id = e.Id;
                    Description = e.Description;
                    break;
                case Events.BooksChanged e:
                    Id = e.Id;
                    Books = e.Books;
                    break;
                case Events.SeriesDeleted e:
                    Id = e.Id;
                    break;
            }
        }
    }
}