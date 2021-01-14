using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.Enums;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using BookOrganizer2.UI.Wpf.Enums;
using BookOrganizer2.UI.Wpf.Events;
using BookOrganizer2.UI.Wpf.Interfaces;
using BookOrganizer2.UI.Wpf.Wrappers;
using Prism.Commands;
using Prism.Events;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public abstract class BaseDetailViewModel<T, TId, TBase> : ViewModelBase, IDetailViewModel
                where TBase : BaseWrapper<T, TId>
                where T : class, IIdentifiable<TId>
                where TId : ValueObject
    {
        protected readonly IEventAggregator EventAggregator;
        protected readonly ILogger Logger;
        protected readonly IDomainService<T, TId> DomainService;
        protected readonly IDialogService DialogService;
        private Tuple<bool, DetailViewState, SolidColorBrush, bool> _userMode;
        private bool _hasChanges;
        private Guid _selectedBookId;
        private string _tabTitle;
        private Guid _id;

        public BaseDetailViewModel(IEventAggregator eventAggregator,
                                   ILogger logger,
                                   IDomainService<T, TId> domainService,
                                   IDialogService dialogService)
        {
            EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            DomainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SwitchEditableStateCommand = new DelegateCommand(SwitchEditableStateExecute);
            SaveItemCommand = new DelegateCommand(SaveItemExecute, SaveItemCanExecute);
            DeleteItemCommand = new DelegateCommand(DeleteItemExecute, DeleteItemCanExecute)
                .ObservesProperty(() => UserMode);
            CloseDetailViewCommand = new DelegateCommand(CloseDetailViewExecute);
            ShowSelectedBookCommand = new DelegateCommand<Guid?>(OnShowSelectedBookExecute, OnShowSelectedBookCanExecute);

            UserMode = (true, DetailViewState.ViewMode, Brushes.LightGray, false).ToTuple();
        }

        public ICommand SwitchEditableStateCommand { get; }
        public ICommand SaveItemCommand { get; protected set; }
        public ICommand DeleteItemCommand { get; }
        public ICommand CloseDetailViewCommand { get; }
        public ICommand ShowSelectedBookCommand { get; }

        public abstract TBase SelectedItem { get; set; }
        public bool IsNewItem { get; set; }

        public Tuple<bool, DetailViewState, SolidColorBrush, bool> UserMode
        {
            get => _userMode;
            set
            {
                _userMode = value;
                OnPropertyChanged();
            }
        }

        public bool HasChanges
        {
            get => _hasChanges;
            set
            {
                if (_hasChanges == value) return;
                _hasChanges = value;
                OnPropertyChanged();
                ((DelegateCommand)SaveItemCommand).RaiseCanExecuteChanged();
            }
        }

        public virtual string TabTitle
        {
            get
            {
                if (_tabTitle is null)
                    return "";
                if (_tabTitle.Length <= 50)
                    return _tabTitle;
                else
                    return _tabTitle.Substring(0, 50) + "...";
            }
            set
            {
                _tabTitle = value;
                OnPropertyChanged();
            }
        }

        public Guid SelectedBookId
        {
            get => _selectedBookId;
            set
            {
                _selectedBookId = value;
                OnPropertyChanged();
                if (_selectedBookId != Guid.Empty)
                {
                    EventAggregator.GetEvent<OpenItemMatchingSelectedBookIdEvent<Guid>>()
                                   .Publish(_selectedBookId);
                }
            }
        }

        public Guid Id
        {
            get => _id;
            set => _id = value;
        }

        public abstract Task LoadAsync(Guid id);
        public abstract TBase CreateWrapper(T entity);

        public virtual void SwitchEditableStateExecute()
        {
            UserMode = UserMode.Item2 == DetailViewState.ViewMode
                ? (!UserMode.Item1, DetailViewState.EditMode, Brushes.LightGreen, !UserMode.Item4).ToTuple()
                : (!UserMode.Item1, DetailViewState.ViewMode, Brushes.LightGray, !UserMode.Item4).ToTuple();
        }

        protected void SetChangeTracker()
        {
            if (!HasChanges)
            {
                HasChanges = DomainService.HasChanges();
            }
        }
        protected virtual bool SaveItemCanExecute()
            => (!SelectedItem.HasErrors) && (HasChanges || IsNewItem);

        protected virtual async void SaveItemExecute()
        {
            if(!IsNewItem)
            {
                var dialog = new OkCancelViewModel("Save changes?", "You are about to save your changes. This will overwrite the previous version. Are you sure?");

                var result = DialogService.OpenDialog(dialog);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            if (IsNewItem)
            {
                var item = await DomainService.AddNew(SelectedItem.Model);

                await LoadAsync(DomainService.GetId(item.Id));
            }
            else
            {
                DomainService.Update(SelectedItem.Model);
                await SaveAsync();
            }

            EventAggregator.GetEvent<ChangeDetailsViewEvent>()
                .Publish(new ChangeDetailsViewEventArgs
                {
                    Message = CreateChangeMessage(IsNewItem ? DatabaseOperation.ADD : DatabaseOperation.UPDATE),
                    MessageBackgroundColor = Brushes.LawnGreen
                });

            HasChanges = false;
            IsNewItem = false;
        }

        protected abstract string CreateChangeMessage(DatabaseOperation operation);

        private void CloseDetailViewExecute()
        {
            if (DomainService.HasChanges())
            {
                var dialog = new OkCancelViewModel("Close the view?", "You have made changes. Closing will loose all unsaved changes. Are you sure you still want to close this view?");
                var result = DialogService.OpenDialog(dialog);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            EventAggregator.GetEvent<CloseDetailsViewEvent>()
                    .Publish(new CloseDetailsViewEventArgs
                    {
                        Id = DomainService.GetId(SelectedItem.Model.Id),
                        ViewModelName = GetType().Name
                    });
        }

        private bool OnShowSelectedBookCanExecute(Guid? id)
                => (!(id is null) && id != Guid.Empty);

        public virtual void OnShowSelectedBookExecute(Guid? id)
            => SelectedBookId = (Guid)id;

        private bool DeleteItemCanExecute()
            => SelectedItem.Model.Id != default && UserMode.Item4;

        private async void DeleteItemExecute()
        {
            var dialog = new OkCancelViewModel("Delete item?", "You are about to delete an item. This operation cannot be undone. Are you sure?");
            var result = DialogService.OpenDialog(dialog);

            if (result == DialogResult.No) { }
            else
            {
                await DomainService.RemoveAsync(SelectedItem.Model.Id);
                await SaveAsync();

                CloseDetailViewExecute();

                EventAggregator.GetEvent<ChangeDetailsViewEvent>()
                    .Publish(new ChangeDetailsViewEventArgs
                    {
                        Message = CreateChangeMessage(DatabaseOperation.DELETE),
                        MessageBackgroundColor = Brushes.DimGray
                    });
            }
        }

        private async Task SaveAsync()
            => await DomainService.SaveChanges();
    }
}