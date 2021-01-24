using System.Threading.Tasks;

namespace BookOrganizer2.Domain.Services
{
    public interface ISimpleDomainService<T, in TId> : IDomainService<T, TId> where T : class
    {
        // Format-, Genre-, Language-, NationalityDetailViewModel
        void ResetTracking(T entity) => Repository.ResetTracking(entity);
        ValueTask<T> GetAsync(TId id) => Repository.GetAsync(id);
    }
}
