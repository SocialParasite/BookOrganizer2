using System.Diagnostics;
using BookOrganizer2.Domain.DA;

namespace BookOrganizer2.Domain.Services
{
    public interface IDomainService<T, in TId> where T : class
    {
        IRepository<T, TId> Repository { get; }
        T CreateItem();
    }
}
