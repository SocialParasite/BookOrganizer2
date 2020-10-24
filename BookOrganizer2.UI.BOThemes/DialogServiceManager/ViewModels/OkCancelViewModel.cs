using BookOrganizer2.UI.BOThemes.DialogServiceManager.Enums;
using Prism.Commands;
using System.Windows.Input;

namespace BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels
{
    public class OkCancelViewModel : BaseDialog<DialogResult>
    {
        public OkCancelViewModel(string title, string message) : base(title, message)
        {
            OKCommand = new DelegateCommand<IDialogWindow>(OkExecute);
            CancelCommand = new DelegateCommand<IDialogWindow>(CancelExecute);
        }

        public ICommand OKCommand { get; }
        public ICommand CancelCommand { get; }

        private void OkExecute(IDialogWindow window) 
            => CloseDialogWithResult(window, DialogResult.Yes);

        private void CancelExecute(IDialogWindow window) 
            => CloseDialogWithResult(window, DialogResult.No);
    }
}
