using BookOrganizer2.UI.BOThemes.DialogServiceManager.Enums;
using Prism.Commands;
using System.Windows.Input;

namespace BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels
{
    public class NotificationViewModel : BaseDialog<DialogResult>
    {
        public NotificationViewModel(string title, string message) : base(title, message) 
            => OKCommand = new DelegateCommand<IDialogWindow>(OkExecute);

        public ICommand OKCommand { get; }

        private void OkExecute(IDialogWindow window) 
            => CloseDialogWithResult(window, DialogResult.Undefined);
    }
}
