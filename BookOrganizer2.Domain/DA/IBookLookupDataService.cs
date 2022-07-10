using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.DA.Conditions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface IBookLookupDataService
    {
        Task<IEnumerable<BookLookupItem>> GetBookLookupAsync(string viewModelName, 
            BookMaintenanceFilterCondition bookMaintenanceFilterCondition = BookMaintenanceFilterCondition.NoFilter,
            bool showOnlyBooksNotRead = false, bool showOnlyNotOwnedBooks = false, 
            IList<Guid> genreFilter = null, IList<Guid> formatFilter = null);
        Task<IEnumerable<GenreLookupItem>> GetGenresAsync();
        Task<IEnumerable<FormatLookupItem>> GetFormatsAsync();
    }
}
