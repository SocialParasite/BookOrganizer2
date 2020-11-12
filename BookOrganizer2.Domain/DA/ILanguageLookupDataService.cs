using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface ILanguageLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetLanguageLookupAsync(string viewModelName);
        Task<Guid> GetLanguageId();
        Task<int> GetLanguageCount();
    }
}
