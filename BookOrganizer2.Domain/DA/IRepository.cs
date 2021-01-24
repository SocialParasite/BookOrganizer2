using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface IRepository<T, in TId> where T : class
    {
        ValueTask<T> GetAsync(TId id);
        Task<bool> ExistsAsync(TId id);

        Task AddAsync(T entity);
        void Update(T entity);
        Task RemoveAsync(TId id);
        Task SaveAsync();
        void ResetTracking(T entity);
        bool HasChanges();
    }
}
