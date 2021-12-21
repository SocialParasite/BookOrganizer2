using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.Domain.DA.Conditions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface ISeriesLookupDataService
    {
        Task<IEnumerable<SeriesLookupItem>> GetSeriesLookupAsync(string viewModelName, 
                                                                 SeriesMaintenanceFilterCondition maintenanceFilterCondition = SeriesMaintenanceFilterCondition.NoFilter, 
                                                                 SeriesFilterCondition condition = SeriesFilterCondition.NoFilter);
    }
}
