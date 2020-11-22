using System;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.BookProfile
{
    public class BookReadDate
    {
        public ReadDateId Id { get; set; }
        public DateTime ReadDate { get; set; }

        public BookReadDate() { }
        public BookReadDate(DateTime date)
        {
            Id = new ReadDateId(SequentialGuid.NewSequentialGuid());
            ReadDate = date;
        }
    }
}
