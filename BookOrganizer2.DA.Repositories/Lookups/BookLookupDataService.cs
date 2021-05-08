using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.DA.Conditions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.Shared;

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

        public async Task<IEnumerable<BookLookupItem>> GetFilteredBookLookupAsync(string viewModelName,
            FilterCondition filterCondition,
            IList<GenreLookupItem> genreFilter = null,
            IList<FormatLookupItem> formatFilter = null)
        {
            // TODO: additional filters? How?
            // - genres: all, 1-n genreFilter=null == all?
            // - formats: all, 1-n

            var filter = GetFilterCondition(filterCondition);
            await using var ctx = _contextCreator();
            return await ctx.Books
                .Include(f => f.Formats)
                .Include(r => r.ReadDates)
                .Include(a => a.Authors)
                .Where(filter)
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

            Expression<Func<Book, bool>> GetFilterCondition(FilterCondition condition)
            {
                return condition switch
                {
                    FilterCondition.NoFilter => b => true,
                    FilterCondition.NoDescription => b => string.IsNullOrEmpty(b.Description),
                    FilterCondition.PlaceholderCover => b => b.BookCoverPath.Contains("placeholder"),
                    FilterCondition.NoAuthors => b => b.Authors.Count == 0,
                    FilterCondition.NotRead => b => !b.IsRead,
                    _ => throw new ArgumentOutOfRangeException(nameof(condition), "Invalid filter condition!")
                };
            }
        }

        public async Task<IEnumerable<GenreLookupItem>> GetGenresAsync()
        {
            await using var ctx = _contextCreator();
            return await ctx.Genres
                .AsNoTracking()
                .OrderBy(n => n.Name)
                .Select(n =>
                    new GenreLookupItem
                    {
                        Id = n.Id,
                        Name = n.Name,
                        IsSelected = false
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
