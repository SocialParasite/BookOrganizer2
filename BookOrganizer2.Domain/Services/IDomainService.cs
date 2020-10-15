using System.Diagnostics;
using BookOrganizer2.Domain.DA;

namespace BookOrganizer2.Domain.Services
{
    public interface IDomainService<out T, in TId> where T : class
    {
        //IRepository<T> Repository { get; }
        //T CreateItem(TId id);
    }
}
