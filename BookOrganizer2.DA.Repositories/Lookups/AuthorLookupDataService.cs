using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookOrganizer2.DA.Repositories.Lookups
{
    public class AuthorLookupDataService : IAuthorLookupDataService
    {
        private readonly Func<BookOrganizer2DbContext> _contextCreator;
        private readonly string _placeholderPic;

        public AuthorLookupDataService(Func<BookOrganizer2DbContext> contextCreator, string imagePath)
        {
            _contextCreator = contextCreator;
            _placeholderPic = imagePath;
        }

        public async Task<IEnumerable<LookupItem>> GetAuthorLookupAsync(string viewModelName)
        {
            await using var ctx = _contextCreator();
            return await ctx.Authors
                .AsNoTracking()
                .OrderBy(a => a.LastName)
                .Select(a =>
                    new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember = $"{a.LastName}, {a.FirstName}",
                        Picture = GetPictureThumbnail(a.MugshotPath) ?? _placeholderPic,
                        ViewModelName = viewModelName
                    })
                .ToListAsync();
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
