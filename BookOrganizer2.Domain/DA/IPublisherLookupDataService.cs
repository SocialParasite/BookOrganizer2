using BookOrganizer2.Domain.DA.Conditions;
using BookOrganizer2.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface IPublisherLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetPublisherLookupAsync(string viewModelName, PublisherMaintenanceFilterCondition condition = PublisherMaintenanceFilterCondition.NoFilter);
        Task<int> GetPublisherCount();
    }
}
