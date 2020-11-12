using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface IFormatLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetFormatLookupAsync(string viewModelName);
        Task<Guid> GetFormatId();
        Task<int> GetFormatCount();
    }
}
