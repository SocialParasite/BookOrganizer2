using System;
using System.Threading.Tasks;

namespace BookOrganizer2.UI.Wpf.Interfaces
{
    public interface IDetailViewModel
    {
        Guid Id { get; set; }
        Task LoadAsync(Guid id);
    }
}
