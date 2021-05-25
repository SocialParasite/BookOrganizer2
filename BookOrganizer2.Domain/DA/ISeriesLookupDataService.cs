using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA.Conditions;

namespace BookOrganizer2.Domain.DA
{
    public interface ISeriesLookupDataService
    {
        Task<IEnumerable<SeriesLookupItem>> GetSeriesLookupAsync(string viewModelName);
        Task<IEnumerable<SeriesLookupItem>> GetFilteredSeriesLookupAsync(string viewModelName, SeriesFilterCondition condition);
    }
}
