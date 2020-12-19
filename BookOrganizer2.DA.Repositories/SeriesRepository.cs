using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.Domain.DA;
using Microsoft.EntityFrameworkCore;

namespace BookOrganizer2.DA.Repositories
{
    public class SeriesRepository : BaseRepository<Series, BookOrganizer2DbContext, SeriesId>, ISeriesRepository
    {
        public SeriesRepository(BookOrganizer2DbContext context) : base(context)
        {
        }

        public async Task ChangeReadOrder(Series series, ICollection<ReadOrder> books)
        {
            // TODO: Instalment number
            var modSeries = await LoadAsync(series.Id).ConfigureAwait(false);

            var newBooks = books.Select(s => s.Book)
                .Except(modSeries.Books.Select(a => a.Book)).ToList();

            if (newBooks.Any())
            {
                foreach (var book in newBooks)
                {
                    var nb = Context.Books.SingleOrDefault(b => b.Id == book.Id);
                    var ro = new ReadOrder { Book = nb, Series = modSeries, Instalment = modSeries.Books.Count + 1 };
                    modSeries.Books.Add(ro);
                }
            }

            Context.Update(modSeries);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Series> LoadAsync(SeriesId id)
        {
            if (id != default)
                return await Context.Series
                    .Include(s => s.Books)
                    .ThenInclude(b => b.Series)
                    .SingleOrDefaultAsync(b => b.Id == id);

            return Series.NewSeries;
        }
    }
}
