using BookOrganizer2.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface ISeriesLookupDataService
    {
        Task<IEnumerable<SeriesLookupItem>> GetSeriesLookupAsync(string viewModelName);
    }
}
