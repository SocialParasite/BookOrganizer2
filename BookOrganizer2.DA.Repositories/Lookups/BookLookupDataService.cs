using BookOrganizer2.DA.Repositories.Shared;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.DA.Conditions;
using BookOrganizer2.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookOrganizer2.DA.Repositories.Lookups
{
    public class BookLookupDataService : IBookLookupDataService
    {
        private readonly Func<BookOrganizer2DbContext> _contextCreator;
        private readonly string _placeholderPic;

        public BookLookupDataService(Func<BookOrganizer2DbContext> contextCreator, string imagePath)
        {
            _contextCreator = contextCreator;
            _placeholderPic = imagePath;
        }

        public async Task<IEnumerable<BookLookupItem>> GetBookLookupAsync(string viewModelName,
            BookMaintenanceFilterCondition bookMaintenanceFilterCondition = BookMaintenanceFilterCondition.NoFilter,
            bool showOnlyBooksNotRead = false,
            bool showOnlyNotOwnedBooks = false,
            IList<Guid> genreFilter = null,
            IList<Guid> formatFilter = null)
        {
            var filter = GetFilterCondition(bookMaintenanceFilterCondition);

            Expression<Func<Book, bool>> genreFilterExpression = genreFilter?.Count > 0
                ? b => b.Genres.Any(g => genreFilter.Contains(g.Id))
                : _ => true;

            Expression<Func<Book, bool>> formatFilterExpression = formatFilter?.Count > 0
                ? b => b.Formats.Any(f => formatFilter.Contains(f.Id))
                : _ => true;

            Expression<Func<Book, bool>> onlyNotReadBooksFilter = showOnlyBooksNotRead ? b => !b.IsRead : _ => true;
            Expression<Func<Book, bool>> onlyNotOwnedBooksFilter = showOnlyNotOwnedBooks ? b => b.Formats.Count == 0 : _ => true;

            var combinedFilter = filter.And(onlyNotReadBooksFilter)
                                                       .And(onlyNotOwnedBooksFilter)
                                                       .And(genreFilterExpression)
                                                       .And(formatFilterExpression);
            
            await using var ctx = _contextCreator();
            return await ctx.Books
                .Include(f => f.Formats)
                .Include(r => r.ReadDates)
                .Include(a => a.Authors)
                .Include(g => g.Genres)
                .Where(combinedFilter ?? filter)
                .AsNoTracking()
                .OrderBy(a => a.Title)
                .Select(a =>
                    new BookLookupItem
                    {
                        Id = a.Id,
                        DisplayMember = a.Title,
                        Picture = GetPictureThumbnail(a.BookCoverPath) ?? _placeholderPic,
                        ViewModelName = viewModelName,
                        InfoText = GetInfoText(a),
                        BookStatus = CheckBookStatus(a.IsRead, a.Formats.Count > 0)
                    })
                .ToListAsync();

            Expression<Func<Book, bool>> GetFilterCondition(BookMaintenanceFilterCondition condition)
            {
                return condition switch
                {
                    BookMaintenanceFilterCondition.NoFilter => b => true,
                    BookMaintenanceFilterCondition.NoDescription => b => string.IsNullOrEmpty(b.Description),
                    BookMaintenanceFilterCondition.PlaceholderCover => b => b.BookCoverPath.Contains("placeholder"),
                    BookMaintenanceFilterCondition.NoAuthors => b => b.Authors.Count == 0,
                    BookMaintenanceFilterCondition.NoPublisher => b => b.Publisher.Equals(null),
                    _ => throw new ArgumentOutOfRangeException(nameof(condition), "Invalid filter condition!")
                };
            }
        }

        public async Task<IEnumerable<GenreLookupItem>> GetGenresAsync()
        {
            await using var ctx = _contextCreator();
            return await ctx.Genres
                .AsNoTracking()
                .OrderBy(n => n.Name)
                .Select(n =>
                    new GenreLookupItem
                    {
                        Id = n.Id,
                        Name = n.Name,
                        IsSelected = false
                    })
                .ToListAsync();
        }

        public async Task<IEnumerable<FormatLookupItem>> GetFormatsAsync()
        {
            await using var ctx = _contextCreator();
            return await ctx.Formats
                .AsNoTracking()
                .OrderBy(n => n.Name)
                .Select(n =>
                    new FormatLookupItem
                    {
                        Id = n.Id,
                        Name = n.Name,
                        IsSelected = false
                    })
                .ToListAsync();
        }

        private static string GetInfoText(Book book)
        {
            var owned = "You do not own this book.";
            var read = "You haven't read this book.";

            if (book.Formats.Any())
            {
                var formats = string.Join(", ", book.Formats.Select(p => p.Name));
                owned = $"You own this book ({formats})";
            }

            if (book.IsRead && book.ReadDates.Any())
            {
                var lastReadDate = book.ReadDates.OrderBy(d => d.ReadDate).Last().ReadDate;
                read = $"This book was last read on {lastReadDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)}";
            }
            return $"{owned}\r{read}";
        }

        private static BookStatus CheckBookStatus(bool read, bool owned)
        {
            return read switch
            {
                true when owned => BookStatus.Read | BookStatus.Owned,
                true => BookStatus.Read,
                _ => owned ? BookStatus.Owned : BookStatus.None,
            };
        }

        private static string GetPictureThumbnail(string picturePath)
        {
            //var extension = Path.GetExtension(picturePath);
            var fileName = Path.GetFileNameWithoutExtension(picturePath);
            //var thumbnail = $"{fileName}_thumb{extension}";
            var thumbnail = $"{fileName}_thumb.jpg";
            var filePath = Path.GetDirectoryName(picturePath);
            var thumbPath = $@"{filePath}\{thumbnail}";
            return thumbPath;
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
