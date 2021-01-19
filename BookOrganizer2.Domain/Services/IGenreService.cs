using System;
using System.Threading.Tasks;
using BookOrganizer2.Domain.BookProfile.GenreProfile;

namespace BookOrganizer2.Domain.Services
{
    public interface IGenreService : ISimpleDomainService<Genre, GenreId>
    {
        Task<Genre> AddNew(string name);
    }
}
