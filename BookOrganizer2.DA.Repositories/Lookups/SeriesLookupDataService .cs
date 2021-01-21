using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Shared;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<LookupItem>> GetSeriesLookupAsync(string viewModelName)
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Series
                    .AsNoTracking()
                    .OrderBy(s => s.Name)
                    .Select(s =>
                        new LookupItem
                        {
                            Id = s.Id,
                            DisplayMember = s.Name,
                            Picture = GetPictureThumbnail(s.PicturePath) ?? _placeholderPic,
                            ViewModelName = viewModelName,
                            //ItemStatus = CheckSeriesStatus(s)
                        })
                    .ToListAsync();
            }


            //SeriesStatus CheckSeriesStatus(Series series)
            //{
            //    var readStatus = series.SeriesReadOrder.All(b => b.Book.IsRead);

            //    bool partlyRead = false;
            //    bool partlyOwned = false;

            //    if (!readStatus) partlyRead = series.SeriesReadOrder.Any(b => b.Book.IsRead);

            //    var owned = series.SeriesReadOrder.All(b => b.Book.FormatLink.Count > 0);

            //    if (!readStatus) partlyOwned = series.SeriesReadOrder.Any(b => b.Book.FormatLink.Count > 0);

            //    if (readStatus && owned)
            //        return SeriesStatus.AllOwnedAllRead;

            //    if (!partlyOwned && !partlyRead)
            //        return SeriesStatus.NoneOwnedNoneRead;

            //    if (!owned && partlyRead)
            //        return SeriesStatus.NoneOwnedPartlyRead;

            //    if (readStatus && !owned)
            //        return SeriesStatus.NoneOwnedAllRead;

            //    if (!partlyRead && partlyOwned)
            //        return SeriesStatus.PartlyOwnedNoneRead;


            //    if (partlyRead && partlyOwned)
            //        return SeriesStatus.PartlyOwnedPartlyRead;

            //    if (readStatus && partlyOwned)
            //        return SeriesStatus.PartlyOwnedAllRead;


            //    if (owned && !partlyRead)
            //        return SeriesStatus.AllOwnedPartlyRead;

            //    if (owned && partlyRead)
            //        return SeriesStatus.AllOwnedPartlyRead;

            //    return SeriesStatus.None;
            //}
        }

        private static string GetPictureThumbnail(string picturePath)
        {
            var extension = Path.GetExtension(picturePath);
            var fileName = Path.GetFileNameWithoutExtension(picturePath);
            //var thumbnail = $"{fileName}_thumb{extension}";
            var thumbnail = $"{fileName}_thumb.jpg";
            var filePath = Path.GetDirectoryName(picturePath);
            var thumbPath = $@"{filePath}\{thumbnail}";
            return thumbPath;
        }
    }
}
