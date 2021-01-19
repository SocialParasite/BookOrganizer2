using BookOrganizer2.Domain.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA.Reports
{
    public interface IAnnualBookStatisticsLookupDataService
    {
        Task<IEnumerable<AnnualBookStatisticsReport>> GetAnnualBookStatisticsReportAsync(int? year = null);
    }
}
