using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;

namespace BookOrganizer2.DA.Repositories
{
    public class LanguageRepository : BaseRepository<Language, BookOrganizer2DbContext, LanguageId>
    {
        public LanguageRepository(BookOrganizer2DbContext context) : base(context) { }

    }
}
