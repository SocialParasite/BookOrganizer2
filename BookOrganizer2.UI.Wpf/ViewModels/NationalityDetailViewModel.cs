using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.Enums;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using BookOrganizer2.UI.Wpf.Enums;
using BookOrganizer2.UI.Wpf.Wrappers;
using Prism.Commands;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using BookOrganizer2.UI.Wpf.Events;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class NationalityDetailViewModel : BaseDetailViewModel<Nationality, NationalityId, NationalityWrapper>
    {
        private NationalityWrapper _selectedItem;
        private readonly INationalityLookupDataService _nationalityLookupDataService;

        public NationalityDetailViewModel(IEventAggregator eventAggregator,
            ILogger logger,
            IDomainService<Nationality, NationalityId> domainService,
            INationalityLookupDataService nationalityLookupDataService,
            IDialogService dialogService)
            : base(eventAggregator, logger, domainService, dialogService)
        {
            _nationalityLookupDataService = nationalityLookupDataService ?? throw new ArgumentNullException(nameof(nationalityLookupDataService));

            ChangeEditedNationalityCommand = new DelegateCommand<Guid?>(OnChangeEditedNationalityExecute);
            SaveItemCommand = new DelegateCommand(SaveItemExecute, base.SaveItemCanExecute)
                .ObservesProperty(() => SelectedItem.Name);

            SelectedItem = CreateWrapper(domainService.CreateItem());

            Nations = new ObservableCollection<LookupItem>();

            UserMode = (!UserMode.Item1, DetailViewState.EditMode, Brushes.LightGray, !UserMode.Item4).ToTuple();
        }

        public ICommand ChangeEditedNationalityCommand { get; }

        public sealed override NationalityWrapper SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value ?? throw new ArgumentNullException(nameof(SelectedItem));
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LookupItem> Nations { get; }

        protected override string CreateChangeMessage(DatabaseOperation operation) 
            => $"{operation}: {SelectedItem.Name}.";

        public sealed override NationalityWrapper CreateWrapper(Nationality entity) => new NationalityWrapper(entity);

        public override async Task LoadAsync(Guid id)
        {
            try
            {
                Nationality nationality = null;

                if (id != default)
                {
                    nationality = await DomainService.Repository.GetAsync(id) ?? Nationality.NewNationality;
                }
                else
                {
                    nationality = Nationality.NewNationality;
                    IsNewItem = true;
                }

                SelectedItem = CreateWrapper(nationality);

                SelectedItem.PropertyChanged += (s, e) =>
                {
                    if (!HasChanges)
                    {
                        HasChanges = DomainService.Repository.HasChanges();
                    }
                    if (e.PropertyName == nameof(SelectedItem.HasErrors))
                    {
                        ((DelegateCommand)SaveItemCommand).RaiseCanExecuteChanged();
                    }
                    if (e.PropertyName == nameof(SelectedItem.Name)
                        || e.PropertyName == nameof(SelectedItem.Name))
                    {
                        TabTitle = SelectedItem.Name;
                    }
                };
                ((DelegateCommand)SaveItemCommand).RaiseCanExecuteChanged();

                Id = id;

                if (Id != default)
                {
                    TabTitle = SelectedItem.Name;
                }
                else
                {
                    SelectedItem.Name = SelectedItem.Model.Name;
                }

                await InitializeFormatCollection();

                async Task InitializeFormatCollection()
                {
                    if (!Nations.Any() || HasChanges)
                    {
                        Nations.Clear();

                        foreach (var item in await GetNationalityList())
                        {
                            Nations.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationViewModel("Exception", ex.Message);
                DialogService.OpenDialog(dialog);

                Logger.Error("Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.Message, ex.StackTrace);
            }
        }

        protected override async void SaveItemExecute()
        {
            base.SaveItemExecute();
            await LoadAsync(SelectedItem.Id);
            NewItemAdded();
        }

        private async Task<IEnumerable<LookupItem>> GetNationalityList()
            => await _nationalityLookupDataService.GetNationalityLookupAsync(nameof(NationalityDetailViewModel));

        private async void OnChangeEditedNationalityExecute(Guid? nationalityId)
        {
            if (DomainService.Repository.HasChanges())
            {
                var dialog = new OkCancelViewModel("Close the view?", "You have made changes. Changing editable nationality will loose all unsaved changes. Are you sure you still want to switch?");
                var result = DialogService.OpenDialog(dialog);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            DomainService.Repository.ResetTracking(SelectedItem.Model);
            HasChanges = DomainService.Repository.HasChanges();

            if (nationalityId is not null) await LoadAsync((Guid) nationalityId);
        }

        private void NewItemAdded()
        {
            EventAggregator.GetEvent<NewItemEvent>()
                .Publish(new NewItemEventArgs
                {
                    //ItemType = "nationality"
                });
        }
    }
}
