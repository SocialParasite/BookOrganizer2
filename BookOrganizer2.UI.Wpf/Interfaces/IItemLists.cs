using System.Threading.Tasks;

namespace BookOrganizer2.UI.Wpf.Interfaces
{
    public interface IItemLists : ISelectedViewModel
    {
        Task InitializeRepositoryAsync();
    }
}
