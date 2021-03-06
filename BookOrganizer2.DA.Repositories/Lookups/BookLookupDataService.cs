using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.DA;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookOrganizer2.DA.Repositories.Lookups
{
    public class BookLookupDataService : IBookLookupDataService
    {
        private readonly Func<BookOrganizer2DbContext> _contextCreator;
        private readonly string _placeholderPic;

        public BookLookupDataService(Func<BookOrganizer2DbContext> contextCreator, string imagePath)
        {
            _contextCreator = contextCreator;
            _placeholderPic = imagePath;
        }

        public async Task<IEnumerable<BookLookupItem>> GetBookLookupAsync(string viewModelName)
        {
            await using var ctx = _contextCreator();
            return await ctx.Books
                .Include(f => f.Formats)
                .Include(r => r.ReadDates)
                .AsNoTracking()
                .OrderBy(a => a.Title)
                .Select(a =>
                    new BookLookupItem
                    {
                        Id = a.Id,
                        DisplayMember = a.Title,
                        Picture = GetPictureThumbnail(a.BookCoverPath) ?? _placeholderPic,
                        ViewModelName = viewModelName,
                        InfoText = GetInfoText(a),
                        BookStatus = CheckBookStatus(a.IsRead, a.Formats.Count > 0)
                    })
                .ToListAsync();
        }

        private static string GetInfoText(Book book)
        {
            var owned = "You do not own this book.";
            var read = "You haven't read this book.";

            if (book.Formats.Any())
            {
                var formats = string.Join(", ", book.Formats.Select(p => p.Name));
                owned = $"You own this book ({formats})";
            }
            
            if (book.IsRead && book.ReadDates.Any())
            {
                var lastReadDate = book.ReadDates.OrderBy(d => d.ReadDate).Last().ReadDate;
                read = $"This book was last read on {lastReadDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)}";
            }
            return $"{owned}\r{read}";
        }

        static BookStatus CheckBookStatus(bool read, bool owned)
        {
            return read switch
            {
                true when owned => BookStatus.Read | BookStatus.Owned,
                true => BookStatus.Read,
                _ => owned ? BookStatus.Owned : BookStatus.None,
            };
        }

        private static string GetPictureThumbnail(string picturePath)
        {
            //var extension = Path.GetExtension(picturePath);
            var fileName = Path.GetFileNameWithoutExtension(picturePath);
            //var thumbnail = $"{fileName}_thumb{extension}";
            var thumbnail = $"{fileName}_thumb.jpg";
            var filePath = Path.GetDirectoryName(picturePath);
            var thumbPath = $@"{filePath}\{thumbnail}";
            return thumbPath;
        }
    }
}
