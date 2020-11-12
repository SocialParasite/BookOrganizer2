using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookOrganizer2.DA.Repositories.Lookups
{
    public class FormatLookupDataService : IFormatLookupDataService
    {
        private readonly Func<BookOrganizer2DbContext> _contextCreator;

        public FormatLookupDataService(Func<BookOrganizer2DbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<IEnumerable<LookupItem>> GetFormatLookupAsync(string viewModelName)
        {
            await using var ctx = _contextCreator();
            return await ctx.Formats
                .AsNoTracking()
                .OrderBy(n => n.Name)
                .Select(n =>
                    new LookupItem
                    {
                        Id = n.Id,
                        DisplayMember = n.Name,
                        Picture = null,
                        ViewModelName = viewModelName
                    })
                .ToListAsync();
        }

        public async Task<Guid> GetFormatId()
        {
            await using var ctx = _contextCreator();
            return await ctx.Formats
                .AsNoTracking()
                .OrderBy(n => n.Name)
                .Select(n => n.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetFormatCount()
        {
            await using var ctx = _contextCreator();
            return ctx.Formats.Count();
        }
    }
}
