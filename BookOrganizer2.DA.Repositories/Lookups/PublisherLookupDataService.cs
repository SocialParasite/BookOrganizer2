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

        public async Task<IEnumerable<LookupItem>> GetPublisherLookupAsync(string viewModelName, 
            PublisherMaintenanceFilterCondition publisherMaintenanceFilterCondition = PublisherMaintenanceFilterCondition.NoFilter)
        {
            var filter = GetFilterCondition(publisherMaintenanceFilterCondition);

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

            static Expression<Func<Publisher, bool>> GetFilterCondition(PublisherMaintenanceFilterCondition condition)
            {
                return condition switch
                {
                    PublisherMaintenanceFilterCondition.NoFilter => p => true,
                    PublisherMaintenanceFilterCondition.NoDescription => p => string.IsNullOrEmpty(p.Description),
                    PublisherMaintenanceFilterCondition.NoBooks => p => p.Books.Count == 0,
                    PublisherMaintenanceFilterCondition.NoLogoPicture => p => p.LogoPath.Contains("placeholder"),
                    _ => throw new ArgumentOutOfRangeException(nameof(condition), "Invalid filter condition!")
                };
            }
        }

        public async Task<int> GetPublisherCount()
        {
            await using var ctx = _contextCreator();
            return await ctx.Publishers.CountAsync();
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
