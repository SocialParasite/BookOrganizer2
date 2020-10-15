using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BookOrganizer2.DA.Repositories
{
    public class AuthorsRepository : BaseRepository<Author, BookOrganizer2DbContext>
    {
        public AuthorsRepository(BookOrganizer2DbContext context) : base(context)
        {
        }

        public override async Task<Author> GetSelectedAsync(Guid id)
        {
            if (id != default)
                return await Context.Authors
                    //.Include(b => b.Nationality)
                    //.Include(b => b.BooksLink)
                    //.ThenInclude(bl => bl.Book)
                    .FirstOrDefaultAsync(b => b.Id == id);

            return new Author();
        }
    }
}
