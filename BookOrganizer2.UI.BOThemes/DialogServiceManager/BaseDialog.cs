namespace BookOrganizer2.UI.BOThemes.DialogServiceManager
{
    public abstract class BaseDialog<T>
    {
        public string Title { get; }
        public string Message { get; }
        public T DialogResult { get; private set; }

        public BaseDialog() : this(string.Empty, string.Empty)
        {
        }

        public BaseDialog(string title) : this(title, string.Empty)
        {
        }

        public BaseDialog(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public void CloseDialogWithResult(IDialogWindow dialog, T result)
        {
            DialogResult = result;

            if (dialog != null)
            {
                dialog.DialogResult = true;
            }
        }
    }
}
