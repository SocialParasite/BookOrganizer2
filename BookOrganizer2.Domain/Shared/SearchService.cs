﻿using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.Shared
{
    public class SearchService : ISearchService
    {
        private readonly ISearchLookupDataService _searchLookupService;

        public SearchService(ISearchLookupDataService searchLookupService)
        {
            _searchLookupService = searchLookupService;
        }

        public Task<List<SearchResult>> Search(string searchTerm)
        {
            return _searchLookupService.Search(searchTerm);
        }
    }
}
