using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.DA.Conditions;
using BookOrganizer2.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.Domain.PublisherProfile;
using System.Linq.Expressions;

namespace BookOrganizer2.DA.Repositories.Lookups
{
    public class PublisherLookupDataService : IPublisherLookupDataService
    {
        private readonly Func<BookOrganizer2DbContext> _contextCreator;
        private readonly string _placeholderPic;

        public PublisherLookupDataService(Func<BookOrganizer2DbContext> contextCreator, string imagePath)
        {
            _contextCreator = contextCreator;
            _placeholderPic = imagePath;
        }

        public async Task<IEnumerable<LookupItem>> GetPublisherLookupAsync(string viewModelName)
        {
            await using var ctx = _contextCreator();
            return await ctx.Publishers
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .Select(a =>
                    new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember = a.Name,
                        Picture = GetPictureThumbnail(a.LogoPath) ?? _placeholderPic,
                        ViewModelName = viewModelName,
                        InfoText = $"Books: {a.Books.Count}"
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

        public async Task<IEnumerable<LookupItem>> GetFilteredPublisherLookupAsync(string viewModelName, PublisherFilterCondition publisherFilterCondition)
        {
            var filter = GetFilterCondition(publisherFilterCondition);

            await using var ctx = _contextCreator();
            return await ctx.Publishers
                .AsNoTracking()
                .Where(filter)
                .OrderBy(p => p.Name)
                .Select(p =>
                    new LookupItem
                    {
                        Id = p.Id,
                        DisplayMember = p.Name,
                        Picture = GetPictureThumbnail(p.LogoPath) ?? _placeholderPic,
                        ViewModelName = viewModelName,
                        InfoText = $"Books: {p.Books.Count}",
                    })
                .ToListAsync();

            static Expression<Func<Publisher, bool>> GetFilterCondition(PublisherFilterCondition condition)
            {
                return condition switch
                {
                    PublisherFilterCondition.NoFilter => p => true,
                    PublisherFilterCondition.NoDescription => p => string.IsNullOrEmpty(p.Description),
                    PublisherFilterCondition.NoBooks => p => p.Books.Count == 0,
                    PublisherFilterCondition.NoLogoPicture => p => p.LogoPath.Contains("placeholder"),
                    _ => throw new ArgumentOutOfRangeException(nameof(condition), "Invalid filter condition!")
                };
            }
        }
    }
}
