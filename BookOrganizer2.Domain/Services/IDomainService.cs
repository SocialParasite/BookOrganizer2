using BookOrganizer2.Domain.DA;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.Services
{
    public interface IDomainService<T, TId> where T : class
    {
        IRepository<T, TId> Repository { get; }
        T CreateItem();
        Task<T> AddNew(T selectedItemModel);
    }
}
