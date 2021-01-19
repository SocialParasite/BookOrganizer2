using BookOrganizer2.Domain.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA.Reports
{
    public interface IBookStatisticsYearRangeLookupDataService
    {
        Task<IEnumerable<AnnualBookStatisticsInRangeReport>> GetAnnualBookStatisticsInRangeReportAsync(int? startYear, int? endYear);
    }
}
