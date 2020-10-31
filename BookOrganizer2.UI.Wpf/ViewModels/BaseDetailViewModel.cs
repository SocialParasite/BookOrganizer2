using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
//using BookOrganizer2.Domain.AuthorProfile;
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
            //ShowSelectedBookCommand = new DelegateCommand<Guid?>(OnShowSelectedBookExecute, OnShowSelectedBookCanExecute);

            UserMode = (true, DetailViewState.ViewMode, Brushes.LightGray, false).ToTuple();
        }

            public ICommand SwitchEditableStateCommand { get; }
            public ICommand SaveItemCommand { get; protected set; }
            public ICommand DeleteItemCommand { get; }
            public ICommand CloseDetailViewCommand { get; }
            public ICommand ShowSelectedBookCommand { get; }

            public abstract TBase SelectedItem { get; set; }

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
                    if (_hasChanges != value)
                    {
                        _hasChanges = value;
                        OnPropertyChanged();
                        ((DelegateCommand)SaveItemCommand).RaiseCanExecuteChanged();
                    }
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

            //public Guid SelectedBookId
            //{
            //    get => selectedBookId;
            //    set
            //    {
            //        selectedBookId = value;
            //        OnPropertyChanged();
            //        if (selectedBookId != Guid.Empty)
            //        {
            //            eventAggregator.GetEvent<OpenItemMatchingSelectedBookIdEvent<Guid>>()
            //                           .Publish(selectedBookId);
            //        }
            //    }
            //}

            public Guid Id
            {
                get => _id;
                set => _id = value;
            }

            public abstract Task LoadAsync(Guid id);
            public abstract TBase CreateWrapper(T entity);

        //protected void SetChangeTracker()
        //{
        //    if (!HasChanges)
        //    {
        //        HasChanges = domainService.Repository.HasChanges();
        //    }
        //}

        public virtual void SwitchEditableStateExecute()
        {
            if (UserMode.Item2 == DetailViewState.ViewMode)
                UserMode = (!UserMode.Item1, DetailViewState.EditMode, Brushes.LightGreen, !UserMode.Item4).ToTuple();
            else
                UserMode = (!UserMode.Item1, DetailViewState.ViewMode, Brushes.LightGray, !UserMode.Item4).ToTuple();
        }

        private void CloseDetailViewExecute()
        {
            if (DomainService.Repository.HasChanges())
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

        //public virtual void OnShowSelectedBookExecute(Guid? id)
        //    => SelectedBookId = (Guid)id;

        // TODO:
        protected virtual bool SaveItemCanExecute()
            => (!SelectedItem.HasErrors) && (HasChanges || SelectedItem.Id == default);

        protected async void SaveItemExecute()
        {
            var isNewItem = false;

            if (SelectedItem.Model.Id != default)
            {
                var dialog = new OkCancelViewModel("Save changes?", "You are about to save your changes. This will overwrite the previous version. Are you sure?");

                var result = DialogService.OpenDialog(dialog);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            if (SelectedItem.Model.Id == default)
            {
                isNewItem = true;
                var author = await DomainService.AddNew(SelectedItem.Model);

                await LoadAsync(DomainService.GetId(author.Id));
            }
            else
            {
                DomainService.Repository.Update(SelectedItem.Model);
                await SaveRepository();
            }
            
            EventAggregator.GetEvent<ChangeDetailsViewEvent>()
                .Publish(new ChangeDetailsViewEventArgs
                {
                    Message = CreateChangeMessage(isNewItem ? DatabaseOperation.ADD : DatabaseOperation.UPDATE),
                    MessageBackgroundColor = Brushes.LawnGreen
                });

            HasChanges = false;
        }

        protected abstract string CreateChangeMessage(DatabaseOperation operation);

        private bool DeleteItemCanExecute()
            => SelectedItem.Model.Id != default && UserMode.Item4;

        private async void DeleteItemExecute()
        {
            var dialog = new OkCancelViewModel("Delete item?", "You are about to delete an item. This operation cannot be undone. Are you sure?");
            var result = DialogService.OpenDialog(dialog);

            if (result == DialogResult.No)
            {
                return;
            }
            else
            {
                await DomainService.Repository.RemoveAsync(SelectedItem.Model.Id);
                await SaveRepository();

                CloseDetailViewExecute();

                EventAggregator.GetEvent<ChangeDetailsViewEvent>()
                    .Publish(new ChangeDetailsViewEventArgs
                    {
                        Message = CreateChangeMessage(DatabaseOperation.DELETE),
                        MessageBackgroundColor = Brushes.DimGray
                    });
            }
        }

        private async Task SaveRepository()
            => await DomainService.Repository.SaveAsync();
    }
}