using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.DA.Reports;
using BookOrganizer2.Domain.Reports;
using Microsoft.EntityFrameworkCore;

namespace BookOrganizer2.DA.Repositories.Lookups
{
    public class ReportLookupDataService : IAnnualBookStatisticsLookupDataService,
        IBookStatisticsYearRangeLookupDataService, IMonthlyReadsLookupDataService
    {
        private readonly Func<BookOrganizer2DbContext> _contextCreator;

        public ReportLookupDataService(Func<BookOrganizer2DbContext> contextCreator)
        {
            _contextCreator = contextCreator ?? throw new ArgumentNullException(nameof(contextCreator));
        }

        public async Task<IEnumerable<AnnualBookStatisticsReport>> GetAnnualBookStatisticsReportAsync(int? year = null)
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Set<AnnualBookStatisticsReport>()
                    .FromSqlInterpolated($"EXEC GetAnnualReadReport {year ?? DateTime.Now.Year}")
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<AnnualBookStatisticsInRangeReport>> GetAnnualBookStatisticsInRangeReportAsync(int? startYear, int? endYear)
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Set<AnnualBookStatisticsInRangeReport>()
                    .FromSqlInterpolated($"EXEC GetPeriodicalReadReport {startYear ?? DateTime.Now.Year}, {endYear ?? DateTime.Now.Year}")
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<MonthlyReadsReport>> GetMonthlyReadsReportAsync(int? year = null, int? month = null)
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Set<MonthlyReadsReport>()
                    .FromSqlInterpolated($"EXEC GetMonthlyReads {year ?? DateTime.Now.Year}, {month ?? DateTime.Now.Month}")
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
    }
}
