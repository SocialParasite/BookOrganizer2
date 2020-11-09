using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface INationalityLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetNationalityLookupAsync(string viewModelName);
        Task<Guid> GetNationalityId();
        Task<int> GetNationalityCount();
    }
}
