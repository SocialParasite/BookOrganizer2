using BookOrganizer2.Domain.BookProfile;

namespace BookOrganizer2.Domain.DA.Reports.DTO
{
    public struct BookWithoutDescription
    {
        public BookId Id { get; set; }
        public string Title { get; set; }
    }
}
