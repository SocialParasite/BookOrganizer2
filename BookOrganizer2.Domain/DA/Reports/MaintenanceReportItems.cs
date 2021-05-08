using System.Collections.Generic;
using BookOrganizer2.Domain.DA.Reports.DTO;

namespace BookOrganizer2.Domain.DA.Reports
{
    public struct MaintenanceReportItems
    {
        public int BookCount { get; set; }
        public int BooksWithoutDescriptionCount { get; set; }
        public ICollection<BookWithoutDescription> BooksWithoutDescription { get; set; }
        //public ICollection<SeriesWithoutBooks> SeriesWithoutBooks { get; set; }
        //public ICollection<BookWithoutCoverPicture> BookWithoutCoverPictures { get; set; }
        //public ICollection<BookWithoutReleaseYear> BookWithoutReleaseYears { get; set; }
        //public ICollection<BookWithoutAuthor> BookWithoutAuthors { get; set; }
        //public ICollection<BookWithoutPageCount> BookWithoutPageCounts { get; set; }
        //public ICollection<NotReadRead> NotReadBooks { get; set; }
        //public ICollection<AuthorWithoutBooks> AuthorsWithoutBooks { get; set; }
        //public ICollection<PublisherWithoutBooks> PublishersWithoutBooks { get; set; }
    }
}