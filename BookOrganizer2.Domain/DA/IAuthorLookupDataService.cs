using BookOrganizer2.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA.Conditions;

namespace BookOrganizer2.Domain.DA
{
    public interface IAuthorLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetAuthorLookupAsync(string viewModelName);

        public Task<IEnumerable<LookupItem>> GetFilteredAuthorLookupAsync(string viewModelName,
            AuthorFilterCondition authorFilterCondition);
    }
}
