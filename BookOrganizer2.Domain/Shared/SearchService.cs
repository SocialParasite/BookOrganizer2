using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;

namespace BookOrganizer2.Domain.Shared
{
    public class SearchService : ISearchService
    {
        private readonly IBookLookupDataService _bookLookupDataService;

        public SearchService(IBookLookupDataService bookLookupDataService)
        {
            _bookLookupDataService = bookLookupDataService;
        }

        public Task<IList<SearchResult>> Search(string searchTerm)
        {
            // search books (booksRepository)
            // - title
            // - notes
            // - description
            

            // TODO: all the rest
            return _bookLookupDataService.Search(searchTerm);
        }
    }
}
