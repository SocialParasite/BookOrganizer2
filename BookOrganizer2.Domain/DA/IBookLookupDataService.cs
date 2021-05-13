using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.DA.Conditions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.Domain.BookProfile.FormatProfile;

namespace BookOrganizer2.Domain.DA
{
    public interface IBookLookupDataService
    {
        Task<IEnumerable<BookLookupItem>> GetBookLookupAsync(string viewModelName);
        Task<IEnumerable<BookLookupItem>> GetFilteredBookLookupAsync(string viewModelName, FilterCondition filterCondition,
            IList<Guid> genreFilter, IList<Guid> formatFilter);
        Task<IEnumerable<GenreLookupItem>> GetGenresAsync();
        Task<IEnumerable<FormatLookupItem>> GetFormatsAsync();
    }
}
