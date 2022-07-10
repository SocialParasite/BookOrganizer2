using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA;

namespace BookOrganizer2.DA.Repositories.Lookups
{
    public class SearchLookupDataService : ISearchLookupDataService
    {
        private readonly Func<BookOrganizer2DbContext> _contextCreator;
        private readonly string _placeholderPic;

        public SearchLookupDataService(Func<BookOrganizer2DbContext> contextCreator, string imagePath)
        {
            _contextCreator = contextCreator;
            _placeholderPic = imagePath;
        }

        public async Task<IList<SearchResult>> Search(string searchTerm)
        {
            await using var ctx = _contextCreator();
            var test = ctx.Books
                .Where(b => b.Description.Contains(searchTerm)
                            || b.Title.Contains(searchTerm)
                            || b.Notes.Any(n => n.Title.Contains(searchTerm)
                                                || n.Content.Contains(searchTerm)))
                .Select(a =>
                    new SearchResult
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Content = a.Description, // TODO:
                        ParentType = "Book"
                    });

            return test.ToList();
        }
    }
}
