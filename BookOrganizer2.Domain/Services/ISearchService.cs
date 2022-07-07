using BookOrganizer2.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.Services
{
    public interface ISearchService
    {
        Task<IList<SearchResult>> Search(string searchTerm);
    }
}
