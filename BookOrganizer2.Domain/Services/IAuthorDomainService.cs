using System;
using System.Threading.Tasks;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using BookOrganizer2.Domain.DA;

namespace BookOrganizer2.Domain.Services
{
    public interface IAuthorDomainService : IDomainService<Author, AuthorId>
    {
        Task<Author> LoadAsync(AuthorId id) => ((IAuthorRepository)Repository).LoadAsync(id);

        ValueTask<Nationality> GetNationalityAsync(Guid id) => ((IAuthorRepository)Repository).GetNationalityAsync(id);
    }
}
