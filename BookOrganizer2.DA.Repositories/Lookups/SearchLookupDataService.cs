using BookOrganizer2.DA.Repositories.Shared;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookOrganizer2.DA.Repositories.Lookups;

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
        var results = ctx.Books
            .Where(b => b.Description.Contains(searchTerm)
                   || b.Title.Contains(searchTerm)
                   || b.Notes.Any(n => n.Title.Contains(searchTerm)
                                       || n.Content.Contains(searchTerm)))
            .Select(a =>
                new SearchResult
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = GetContent(a, searchTerm),
                    ParentType = "Book",
                    Picture = a.BookCoverPath,
                    ViewModelName = "BookDetailViewModel" // HACK
                });

        return results.ToList();
    }

    private static string GetContent(Book book, string searchTerm)
    {
        if (book.Notes.Any(n => n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
        {
            var note = book.Notes.First(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Title.EquallyDividedSubstring(searchTerm);
            return note;
        }

        if (book.Notes.Any(n => n.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
        {
            var note =  book.Notes.First(b => b.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Content.EquallyDividedSubstring(searchTerm);
            return note;
        }

        if (book.Description is not null && book.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
        {
            return book.Description.EquallyDividedSubstring(searchTerm);
        }

        return book.Title;
    }
}