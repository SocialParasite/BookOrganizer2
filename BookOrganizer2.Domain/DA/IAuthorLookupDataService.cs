using BookOrganizer2.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA.Conditions;

namespace BookOrganizer2.Domain.DA
{
    public interface IAuthorLookupDataService
    {
        public Task<IEnumerable<LookupItem>> GetAuthorLookupAsync(string viewModelName,
            AuthorMaintenanceFilterCondition authorMaintenanceFilterCondition = AuthorMaintenanceFilterCondition.NoFilter);

        public Task<int> GetAuthorCount();
    }
}
