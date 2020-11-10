using System.Threading.Tasks;
using BookOrganizer2.Domain.PublisherProfile;

namespace BookOrganizer2.Domain.DA
{
    public interface IPublisherRepository : IRepository<Publisher, PublisherId>
    {
        Task<Publisher> LoadAsync(PublisherId id);
    }
}
