using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using BookOrganizer2.Domain.DA;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookOrganizer2.DA.Repositories
{
    public class AuthorRepository : BaseRepository<Author, BookOrganizer2DbContext, AuthorId>, IAuthorRepository
    {
        public AuthorRepository(BookOrganizer2DbContext context) : base(context)
        {
        }

        public async Task<Author> LoadAsync(AuthorId id)
        {
            if (id != default)
            {
                return await Context.Authors
                    .Include(b => b.Nationality)
                    .Include(b => b.Books)
                    .Include(n => n.Notes)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }

            return Author.NewAuthor;
        }
        public async Task ChangeNationality(Author a, NationalityId nationalityId)
        {
            var author = await Context.Authors.FindAsync(a.Id);
            var nationality = await GetNationalityAsync(nationalityId);
            author.SetNationality(nationality);
            await Context.SaveChangesAsync();
        }

        public ValueTask<Nationality> GetNationalityAsync(NationalityId nationalityId) 
            => Context.Nationalities.FindAsync(nationalityId);
    }
}
