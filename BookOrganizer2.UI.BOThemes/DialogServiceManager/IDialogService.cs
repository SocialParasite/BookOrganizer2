namespace BookOrganizer2.UI.BOThemes.DialogServiceManager
{
    public interface IDialogService
    {
        T OpenDialog<T>(BaseDialog<T> viewModel);
    }
}
