using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.DA.Conditions;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.Domain.BookProfile.FormatProfile;

namespace BookOrganizer2.Domain.DA
{
    public interface IBookLookupDataService
    {
        Task<IEnumerable<BookLookupItem>> GetBookLookupAsync(string viewModelName);
        Task<IEnumerable<BookLookupItem>> GetFilteredBookLookupAsync(string viewModelName, FilterCondition filterCondition,
            IList<GenreLookupItem> genreFilter, IList<FormatLookupItem> formatFilter);
        Task<IEnumerable<GenreLookupItem>> GetGenresAsync();
    }
}
