using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA
{
    public interface IRepository<T> where T : class
    {
        void Update(T entity);
        Task SaveAsync();
    }
}
