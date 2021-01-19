namespace BookOrganizer2.Domain.Reports
{
    public class AnnualBookStatisticsInRangeReport
    {
        public int Year { get; set; }
        public int TotalNumberOfBooksRead { get; set; }
        public int ShortestBookRead { get; set; }
        public int LongestBookRead { get; set; }
        public int TotalPagesRead { get; set; }
        public int AveragePagesReadMonthly { get; set; }
        public int AverageBookLength { get; set; }
    }
}
