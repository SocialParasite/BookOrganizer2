using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface IRepository<T, in TId> where T : class
    {
        void Update(T entity);
        Task<T> GetAsync(TId id);
        Task SaveAsync();
        Task<bool> ExistsAsync(TId id);
        Task AddAsync(T entity);
        Task RemoveAsync(TId id);
        bool HasChanges();
    }
}
