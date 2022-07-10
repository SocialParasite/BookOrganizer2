﻿using BookOrganizer2.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface ISearchLookupDataService
    {
        Task<IList<SearchResult>> Search(string searchTerm);
    }
}
