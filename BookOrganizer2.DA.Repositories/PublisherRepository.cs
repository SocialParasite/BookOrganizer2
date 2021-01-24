using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.Domain.DA;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookOrganizer2.DA.Repositories
{
    public class PublisherRepository : BaseRepository<Publisher, BookOrganizer2DbContext, PublisherId>, IPublisherRepository
    {
        public PublisherRepository(BookOrganizer2DbContext context) : base(context)
        {
        }

        public async Task<Publisher> LoadAsync(PublisherId id)
        {
            if (id != default)
            {
                return await Context.Publishers
                    .Include(b => b.Books)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }

            return Publisher.NewPublisher;
        }
    }
}
