using System;

namespace BookOrganizer2.Domain.Reports
{
    public sealed class MonthlyReadsReport
    {
        public string Title { get; set; }
        public DateTime ReadDate { get; set; }
    }
}
