using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;

namespace BookOrganizer2.DA.Repositories
{
    public class NationalityRepository : BaseRepository<Nationality, BookOrganizer2DbContext, NationalityId>
    {
        public NationalityRepository(BookOrganizer2DbContext context) : base(context) { }

    }
}
