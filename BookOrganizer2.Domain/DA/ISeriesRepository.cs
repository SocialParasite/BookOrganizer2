using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;

namespace BookOrganizer2.Domain.DA
{
    public interface ISeriesRepository : IRepository<Series, SeriesId>
    {
        Task<Series> LoadAsync(SeriesId id);

        Task ChangeReadOrder(Series series, ICollection<ReadOrder> books);
    }
}
