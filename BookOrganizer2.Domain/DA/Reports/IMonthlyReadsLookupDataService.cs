﻿using BookOrganizer2.Domain.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA.Reports
{
    public interface IMonthlyReadsLookupDataService
    {
        Task<IEnumerable<MonthlyReadsReport>> GetMonthlyReadsReportAsync(int? year = null, int? month = null);
    }
}
