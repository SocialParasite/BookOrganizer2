using BookOrganizer2.Domain.DA;

namespace BookOrganizer2.Domain.Services
{
    public interface IDomainService<T> where T : class
    {
        IRepository<T> Repository { get; }
        T CreateItem();
    }
}
