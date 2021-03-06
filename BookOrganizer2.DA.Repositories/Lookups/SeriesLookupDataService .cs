using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.Domain.DA;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookOrganizer2.DA.Repositories.Lookups
{
    public class SeriesLookupDataService : ISeriesLookupDataService
    {
        private readonly Func<BookOrganizer2DbContext> _contextCreator;
        private readonly string _placeholderPic;

        public SeriesLookupDataService(Func<BookOrganizer2DbContext> contextCreator, string imagePath)
        {
            _contextCreator = contextCreator;
            _placeholderPic = imagePath;
        }

        public async Task<IEnumerable<SeriesLookupItem>> GetSeriesLookupAsync(string viewModelName)
        {
            await using var ctx = _contextCreator();
            return await ctx.Series
                .Include(b => b.Books)
                .ThenInclude(b => b.Book)
                .ThenInclude(b => b.Formats)
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s =>
                    new SeriesLookupItem
                    {
                        Id = s.Id,
                        DisplayMember = s.Name,
                        Picture = GetPictureThumbnail(s.PicturePath) ?? _placeholderPic,
                        ViewModelName = viewModelName,
                        InfoText = GetInfoText(s),
                        SeriesState = GetSeriesState(s)
                    })
                .ToListAsync();
        }

        private static string GetInfoText(Series s) 
            => $"Owned: {GetOwnedBooks(s)} of {GetBookCount(s)} \rRead: {GetReadBooks(s)} of {GetBookCount(s)}";

        private static SeriesState GetSeriesState(Series s)
        {
            return new()
            {
                BookCount = GetBookCount(s),
                OwnedBookCount = GetOwnedBooks(s),
                ReadBookCount = GetReadBooks(s)
            };
        }

        private static int GetReadBooks(Series series) => series.Books.Count(b => b.Book.IsRead) / 2;

        private static int GetOwnedBooks(Series series) => series.Books.Count(b => b.Book.Formats.Any()) / 2;

        private static int GetBookCount(Series series) => series.Books.Count / 2;

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
