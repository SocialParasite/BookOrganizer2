using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface IGenreLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetGenreLookupAsync(string viewModelName);
        Task<Guid> GetGenreId();
        Task<int> GetGenreCount();
    }
}
