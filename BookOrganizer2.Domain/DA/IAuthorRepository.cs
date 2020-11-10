using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface IAuthorRepository : IRepository<Author, AuthorId>
    {
        Task<Author> LoadAsync(AuthorId id);
        Task ChangeNationality(Author author, NationalityId nationalityId);
        Task<Nationality> GetNationalityAsync(NationalityId id);
    }
}
