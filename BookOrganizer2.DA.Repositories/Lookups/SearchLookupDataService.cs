using BookOrganizer2.DA.Repositories.Shared;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookOrganizer2.DA.Repositories.Lookups;

public class SearchLookupDataService : ISearchLookupDataService
{
    private readonly Func<BookOrganizer2DbContext> _contextCreator;

    public SearchLookupDataService(Func<BookOrganizer2DbContext> contextCreator)
    {
        _contextCreator = contextCreator;
    }

    public async Task<List<SearchResult>> Search(string searchTerm)
    {
        var result = new List<SearchResult>();

        var books = SearchBooks(searchTerm);
        var authors = SearchAuthors(searchTerm);

        await Task.WhenAll(books, authors);
        
        result.AddRange(await books);
        result.AddRange(await authors);

        return result;
    }

    private async Task<List<SearchResult>> SearchBooks(string searchTerm)
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
                })
            .AsNoTracking();

        return results.ToList();
    }

    private async Task<List<SearchResult>> SearchAuthors(string searchTerm)
    {
        await using var ctx = _contextCreator();
        var results = ctx.Authors
            .Where(a => a.Biography.Contains(searchTerm)
                        || a.FirstName.Contains(searchTerm)
                        || a.LastName.Contains(searchTerm)
                        || a.Nationality.Name.Contains(searchTerm)
                        || a.Notes.Any(n => n.Title.Contains(searchTerm)
                                            || n.Content.Contains(searchTerm)))
            .Select(a =>
                new SearchResult
                {
                    Id = a.Id,
                    Title = $"{a.LastName}, {a.FirstName}",
                    Content = GetAuthorContent(a, searchTerm),
                    ParentType = "Author",
                    Picture = a.MugshotPath,
                    ViewModelName = "AuthorDetailViewModel" // HACK
                })
            .AsNoTracking();

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
            var note = book.Notes.First(b => b.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Content.EquallyDividedSubstring(searchTerm);
            return note;
        }

        if (book.Description is not null && book.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
        {
            return book.Description.EquallyDividedSubstring(searchTerm);
        }

        return book.Title;
    }

    private static string GetAuthorContent(Author author, string searchTerm)
    {
        if (author.Notes.Any(n => n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
        {
            var note = author.Notes.First(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Title.EquallyDividedSubstring(searchTerm);
            return note;
        }

        if (author.Notes.Any(n => n.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
        {
            var note = author.Notes.First(b => b.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Content.EquallyDividedSubstring(searchTerm);
            return note;
        }

        if (author.Biography is not null && author.Biography.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
        {
            return author.Biography.EquallyDividedSubstring(searchTerm);
        }

        return $"{author.LastName}, {author.FirstName}";
    }
}