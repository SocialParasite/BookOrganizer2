using BookOrganizer2.DA.Repositories;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using BookOrganizer2.UI.Wpf.Enums;
using BookOrganizer2.UI.Wpf.Events;
using BookOrganizer2.UI.Wpf.Services;
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

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class AuthorDetailViewModel : BaseDetailViewModel<Author, AuthorId, AuthorWrapper>
    {
        private LookupItem _selectedNationality;
        private AuthorWrapper _selectedItem;
        private bool _nationalityIsDirty;

        private bool NationalityIsDirty
        {
            get => _nationalityIsDirty;
            set { _nationalityIsDirty = value; OnPropertyChanged(); }
        }

        public AuthorDetailViewModel(IEventAggregator eventAggregator,
                                     ILogger logger,
                                     IDomainService<Author, AuthorId> domainService,
                                     IDialogService dialogService)
            : base(eventAggregator, logger, domainService, dialogService)
        {
            AddAuthorPictureCommand = new DelegateCommand(OnAddAuthorPictureExecute);
            AddNewNationalityCommand = new DelegateCommand(OnAddNewNationalityExecute);
            NationalitySelectionChangedCommand = new DelegateCommand(OnNationalitySelectionChangedExecute);

            SaveItemCommand = new DelegateCommand(SaveItemExecute, SaveItemCanExecute)
                .ObservesProperty(() => SelectedItem.FirstName)
                .ObservesProperty(() => SelectedItem.LastName)
                .ObservesProperty(() => NationalityIsDirty);
            SelectedItem = new AuthorWrapper(domainService.CreateItem());

            Nationalities = new ObservableCollection<LookupItem>();

            eventAggregator.GetEvent<NewItemEvent>()
                .Subscribe(OnNewNationalityAdded);
        }

        private async void OnNewNationalityAdded(NewItemEventArgs obj) 
            => await InitializeNationalityCollection(true);

        public ICommand AddAuthorPictureCommand { get; }
        public ICommand NationalitySelectionChangedCommand { get; }
        public ICommand AddNewNationalityCommand { get; }

        public LookupItem SelectedNationality
        {
            get => _selectedNationality;
            set { _selectedNationality = value; OnPropertyChanged(); }
        }

        public ObservableCollection<LookupItem> Nationalities { get; set; }

        public sealed override AuthorWrapper SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value ?? throw new ArgumentNullException(nameof(SelectedItem));
                OnPropertyChanged();
            }
        }
        public override AuthorWrapper CreateWrapper(Author entity) => new AuthorWrapper(entity);

        public override async Task LoadAsync(Guid id)
        {
            try
            {
                Author author = null;

                if (id != default)
                {
                    author = await ((AuthorRepository)DomainService.Repository).LoadAsync(id);
                }
                else
                {
                    author = Author.NewAuthor;
                    IsNewItem = true;
                }


                SelectedItem = CreateWrapper(author);

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
                    if (e.PropertyName == nameof(SelectedItem.FirstName)
                        || e.PropertyName == nameof(SelectedItem.LastName))
                    {
                        SetTabTitle();
                    }
                };
                ((DelegateCommand)SaveItemCommand).RaiseCanExecuteChanged();

                Id = id;

                if (Id != default)
                {
                    SetTabTitle();
                }
                else
                {
                    SwitchEditableStateExecute();
                    SelectedItem.FirstName = "";
                    SelectedItem.LastName = "";
                }

                SetDefaultAuthorPicIfNoneSet();

                InitiliazeSelectedNationalityIfNoneSet();

                void SetDefaultAuthorPicIfNoneSet()
                {
                    SelectedItem.MugshotPath ??= FileExplorerService.GetImagePath();
                }

                void InitiliazeSelectedNationalityIfNoneSet()
                {
                    if (SelectedNationality is null && SelectedItem.Model.Nationality is not null)
                    {
                        SelectedNationality =
                            new LookupItem
                            {
                                Id = SelectedItem.Model.Nationality.Id,
                                DisplayMember = SelectedItem.Model.Nationality.Name
                            };
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

        public override async void SwitchEditableStateExecute()
        {
            base.SwitchEditableStateExecute();

            await InitializeNationalityCollection().ConfigureAwait(false);
        }

        private async Task InitializeNationalityCollection(bool reset = false)
        {
            if (!Nationalities.Any() || reset)
            {
                if (reset)
                {
                    while (await ((AuthorService)DomainService).NationalityLookupDataService.GetNationalityCount() 
                           == Nationalities.Count)
                    {
                    }
                }

                Nationalities.Clear();

                foreach (var item in await GetNationalityList())
                {
                    Nationalities.Add(item);
                }

                if (SelectedItem.Model.Nationality != null)
                    SelectedNationality = Nationalities.FirstOrDefault(n => n.Id == SelectedItem.Model.Nationality.Id);
            }
        }

        protected override bool SaveItemCanExecute()
        {
            return (!SelectedItem.HasErrors) && (HasChanges || IsNewItem || NationalityIsDirty);
        }

        protected override async void SaveItemExecute()
        {
            if (_nationalityIsDirty)
            {
                var currentNationality =
                    await ((AuthorRepository)DomainService.Repository).GetNationalityAsync(SelectedNationality.Id);
                SelectedItem.Model.SetNationality(currentNationality);
            }
            base.SaveItemExecute();
        }

        protected override string CreateChangeMessage(DatabaseOperation operation)
            => $"{operation}: {SelectedItem.LastName}, {SelectedItem.FirstName}.";

        private void SetTabTitle()
            => TabTitle = $"{SelectedItem.Model.LastName}, {SelectedItem.Model.FirstName}";

        private void OnAddAuthorPictureExecute()
        {
            var temp = SelectedItem.MugshotPath;
            SelectedItem.MugshotPath = FileExplorerService.BrowsePicture() ?? SelectedItem.MugshotPath;
            if (!string.IsNullOrEmpty(SelectedItem.MugshotPath)
                && SelectedItem.MugshotPath != temp)
            {
                FileExplorerService.CreateThumbnail(SelectedItem.MugshotPath);
            }
        }

        private async Task<IEnumerable<LookupItem>> GetNationalityList()
            => await ((AuthorService)DomainService).NationalityLookupDataService
                                                     .GetNationalityLookupAsync(nameof(NationalityDetailViewModel));

        private void OnNationalitySelectionChangedExecute()
        {
            if (SelectedNationality is not null && Nationalities.Any())
            {
                NationalityIsDirty = SelectedItem.Model.Nationality is null ||
                                     SelectedItem.Model.Nationality?.Id != SelectedNationality.Id;
            }
        }

        private void OnAddNewNationalityExecute()
        {
            EventAggregator.GetEvent<OpenDetailViewEvent>()
                           .Publish(new OpenDetailViewEventArgs
                           {
                               Id = Guid.Empty,
                               ViewModelName = nameof(NationalityDetailViewModel)
                           });
        }
    }
}
