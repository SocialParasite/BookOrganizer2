namespace BookOrganizer2.Domain.Reports
{
    public class AnnualBookStatisticsReport
    {
        public string MonthName { get; set; }
        public int TotalNumberOfBooksRead { get; set; }
        public int ShortestBookRead { get; set; }
        public int LongestBookRead { get; set; }
        public int TotalPagesRead { get; set; }
        public int AveragePagesReadDaily { get; set; }
        public int AverageBookLength { get; set; }
    }
}
