using System.Linq;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.DA;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;

namespace BookOrganizer2.DA.Repositories
{
    public class SeriesRepository : BaseRepository<Series, BookOrganizer2DbContext, SeriesId>, ISeriesRepository
    {
        public SeriesRepository(BookOrganizer2DbContext context) : base(context)
        {
        }

        public async Task<Series> LoadAsync(SeriesId id)
        {
            if (id != default)
                return await Context.Series
                    .Include(s => s.Books)
                    .FirstOrDefaultAsync(b => b.Id == id);

            return Series.NewSeries;
        }

        public async Task TEST(SeriesId id)
        {
            foreach (var book in Context.Set<ReadOrder>())
            {
                
            }

            var series = await Context.Series
                .Include(s => s.Books)
                .FirstOrDefaultAsync(b => b.Id == id);

            //series.Books.OrderBy()
        }
    }
}
