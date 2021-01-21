using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.Domain.BookProfile;

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

        public async Task<IEnumerable<LookupItem>> GetBookLookupAsync(string viewModelName)
        {
            await using var ctx = _contextCreator();
            return await ctx.Books
                .AsNoTracking()
                .OrderBy(a => a.Title)
                .Select(a =>
                    new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember = a.Title,
                        Picture = GetPictureThumbnail(a.BookCoverPath) ?? _placeholderPic,
                        ViewModelName = viewModelName, 
                        ItemStatus = CheckBookStatus(a.IsRead, a.Formats.Count > 0)
                    })
                .ToListAsync();
        }

        static BookStatus CheckBookStatus(bool read, bool owned)
        {
            switch (read)
            {
                case true when owned:
                    return BookStatus.Read | BookStatus.Owned;
                case true:
                    return BookStatus.Read;
            }

            return owned ? BookStatus.Owned : BookStatus.None;
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
