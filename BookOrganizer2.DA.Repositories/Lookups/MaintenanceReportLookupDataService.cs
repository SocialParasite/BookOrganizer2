using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.DA.Reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA.Reports.DTO;

namespace BookOrganizer2.DA.Repositories.Lookups
{
    public class MaintenanceReportLookupDataService : IMaintenanceReportLookupDataService
    {
        private readonly Func<BookOrganizer2DbContext> _contextCreator;

        public MaintenanceReportLookupDataService(Func<BookOrganizer2DbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<MaintenanceReportItems> GetMaintenanceData()
        {
            await using var ctx = _contextCreator();

            var booksWithoutDescriptions = await ctx.Books
                .AsNoTracking()
                .Where(b => b.Description == null || b.Description == string.Empty)
                .Select(x => new BookWithoutDescription
                    {
                        Id = x.Id,
                        Title = x.Title
                    })
                .ToListAsync();

            var mri = new MaintenanceReportItems
            {
                BookCount = await ctx.Books.CountAsync(),
                BooksWithoutDescriptionCount = await ctx.Books.Where(b => string.IsNullOrEmpty(b.Description)).CountAsync(),
                BooksWithoutDescription = booksWithoutDescriptions
            };

            return mri;
        }
    }
}
