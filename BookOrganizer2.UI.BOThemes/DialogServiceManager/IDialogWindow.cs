namespace BookOrganizer2.UI.BOThemes.DialogServiceManager
{
    public interface IDialogWindow
    {
        bool? DialogResult { get; set; }
        object DataContext { get; set; }

        bool? ShowDialog();
    }
}
