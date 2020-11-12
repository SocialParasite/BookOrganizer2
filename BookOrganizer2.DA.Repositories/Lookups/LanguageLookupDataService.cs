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
    public class LanguageLookupDataService : ILanguageLookupDataService
    {
        private readonly Func<BookOrganizer2DbContext> _contextCreator;

        public LanguageLookupDataService(Func<BookOrganizer2DbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<IEnumerable<LookupItem>> GetLanguageLookupAsync(string viewModelName)
        {
            await using var ctx = _contextCreator();
            return await ctx.Languages
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

        public async Task<Guid> GetLanguageId()
        {
            await using var ctx = _contextCreator();
            return await ctx.Languages
                .AsNoTracking()
                .OrderBy(n => n.Name)
                .Select(n => n.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetLanguageCount()
        {
            await using var ctx = _contextCreator();
            return ctx.Languages.Count();
        }
    }
}
