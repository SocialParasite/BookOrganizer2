using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface ISeriesLookupDataService
    {
        Task<IEnumerable<SeriesLookupItem>> GetSeriesLookupAsync(string viewModelName);
    }
}
