using BookOrganizer2.DA.Repositories.Shared;
using BookOrganizer2.DA.SqlServer;
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
        var publishers = SearchPublishers(searchTerm);
        var series = SearchSeries(searchTerm);

        result.AddRange(await books);
        result.AddRange(await authors);
        result.AddRange(await publishers);
        result.AddRange(await series);

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
                    Content = DataHelpers.GetBookContent(a, searchTerm, 100),
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
                    Content = DataHelpers.GetAuthorContent(a, searchTerm, 100),
                    ParentType = "Author",
                    Picture = a.MugshotPath,
                    ViewModelName = "AuthorDetailViewModel" // HACK
                })
            .AsNoTracking();

        return results.ToList();
    }

    private async Task<List<SearchResult>> SearchPublishers(string searchTerm)
    {
        await using var ctx = _contextCreator();
        var results = ctx.Publishers
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .Select(p =>
                new SearchResult
                {
                    Id = p.Id,
                    Title = $"{p.Name}",
                    Content = DataHelpers.GetPublisherContent(p, searchTerm, 100),
                    ParentType = "Publisher",
                    Picture = p.LogoPath,
                    ViewModelName = "PublisherDetailViewModel" // HACK
                })
            .AsNoTracking();

        return results.ToList();
    }

    private async Task<List<SearchResult>> SearchSeries(string searchTerm)
    {
        await using var ctx = _contextCreator();
        var results = ctx.Series
            .Where(s => s.Name.Contains(searchTerm) || s.Description.Contains(searchTerm))
            .Select(s =>
                new SearchResult
                {
                    Id = s.Id,
                    Title = $"{s.Name}",
                    Content = DataHelpers.GetSeriesContent(s, searchTerm, 100),
                    ParentType = "Series",
                    Picture = s.PicturePath,
                    ViewModelName = "SeriesDetailViewModel" // HACK
                })
            .AsNoTracking();

        return results.ToList();
    }
}