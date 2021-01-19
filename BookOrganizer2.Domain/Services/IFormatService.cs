using System.Threading.Tasks;
using BookOrganizer2.Domain.BookProfile.FormatProfile;

namespace BookOrganizer2.Domain.Services
{
    public interface IFormatService : ISimpleDomainService<Format, FormatId>
    {
        Task<Format> AddNew(string name);
    }
}
