using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile.FormatProfile;

namespace BookOrganizer2.DA.Repositories
{
    public class FormatRepository : BaseRepository<Format, BookOrganizer2DbContext, FormatId>
    {
        public FormatRepository(BookOrganizer2DbContext context) : base(context) { }

    }
}
