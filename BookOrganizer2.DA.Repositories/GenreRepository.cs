using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile.GenreProfile;

namespace BookOrganizer2.DA.Repositories
{
    public class GenreRepository : BaseRepository<Genre, BookOrganizer2DbContext, GenreId>
    {
        public GenreRepository(BookOrganizer2DbContext context) : base(context) { }

    }
}
