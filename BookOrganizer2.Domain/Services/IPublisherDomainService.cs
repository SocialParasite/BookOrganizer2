using System.Threading.Tasks;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.PublisherProfile;

namespace BookOrganizer2.Domain.Services
{
    public interface IPublisherDomainService : IDomainService<Publisher, PublisherId>
    {
        Task<Publisher> LoadAsync(PublisherId id) => ((IPublisherRepository)Repository).LoadAsync(id);
    }
}
