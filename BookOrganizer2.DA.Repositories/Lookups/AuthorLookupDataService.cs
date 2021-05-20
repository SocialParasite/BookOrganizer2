using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.DA.Conditions;

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
                        ViewModelName = viewModelName,
                        InfoText = $"Books: {a.Books.Count}"
                    })
                .ToListAsync();
        }

        public async Task<IEnumerable<LookupItem>> GetFilteredAuthorLookupAsync(string viewModelName, AuthorFilterCondition authorFilterCondition)
        {
            var filter = GetFilterCondition(authorFilterCondition);

            await using var ctx = _contextCreator();
            return await ctx.Authors
                .AsNoTracking()
                .Where(filter)
                .OrderBy(a => a.LastName)
                .Select(a =>
                    new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember = $"{a.LastName}, {a.FirstName}",
                        Picture = GetPictureThumbnail(a.MugshotPath) ?? _placeholderPic,
                        ViewModelName = viewModelName,
                        InfoText = $"Books: {a.Books.Count}",
                    })
                .ToListAsync();

            static Expression<Func<Author, bool>> GetFilterCondition(AuthorFilterCondition condition)
            {
                return condition switch
                {
                    AuthorFilterCondition.NoFilter => a => true,
                    AuthorFilterCondition.NoBio => a => string.IsNullOrEmpty(a.Biography),
                    AuthorFilterCondition.NoBooks => a => a.Books.Count == 0,
                    AuthorFilterCondition.NoDateOfBirth => a => a.DateOfBirth.Equals((DateTime?)null),
                    AuthorFilterCondition.NoNationality => a => a.Nationality.Equals(null),
                    AuthorFilterCondition.NoMugshot => a => a.MugshotPath.Contains("placeholder"),
                    _ => throw new ArgumentOutOfRangeException(nameof(condition), "Invalid filter condition!")
                };
            }
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
