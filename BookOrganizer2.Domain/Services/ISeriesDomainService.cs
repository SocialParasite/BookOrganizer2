using System;
using System.Threading.Tasks;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.Domain.DA;

namespace BookOrganizer2.Domain.Services
{
    public interface ISeriesDomainService : IDomainService<Series, SeriesId>
    {        
        Task<Series> LoadAsync(SeriesId id) => ((ISeriesRepository)Repository).LoadAsync(id);

        Task<Book> GetBookAsync(Guid id) => ((ISeriesRepository) Repository).GetBookAsync(id);
    }
}
