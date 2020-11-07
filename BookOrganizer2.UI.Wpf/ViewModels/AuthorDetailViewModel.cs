using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BookOrganizer2.DA.Repositories;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
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

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class AuthorDetailViewModel : BaseDetailViewModel<Author, AuthorId, AuthorWrapper>
    {
        private LookupItem _selectedNationality;
        private AuthorWrapper _selectedItem;
        private bool _nationalityIsDirty;

        public bool NationalityIsDirty
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
            //AddNewNationalityCommand = new DelegateCommand(OnAddNewNationalityExecute);
            NationalitySelectionChangedCommand = new DelegateCommand(OnNationalitySelectionChangedExecute);

            SaveItemCommand = new DelegateCommand(SaveItemExecute, SaveItemCanExecute)
                .ObservesProperty(() => SelectedItem.FirstName)
                .ObservesProperty(() => SelectedItem.LastName)
                .ObservesProperty(() => NationalityIsDirty);
            SelectedItem = new AuthorWrapper(domainService.CreateItem());

            Nationalities = new ObservableCollection<LookupItem>();
        }

        public ICommand AddAuthorPictureCommand { get; }
        public ICommand NationalitySelectionChangedCommand { get; }
        public ICommand AddNewNationalityCommand { get; }

        public LookupItem SelectedNationality
        {
            get => _selectedNationality;
            set { _selectedNationality = value; OnPropertyChanged(); }
        }

        public ObservableCollection<LookupItem> Nationalities { get; set; }

        public override AuthorWrapper SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value ?? throw new ArgumentNullException(nameof(SelectedItem));
                OnPropertyChanged();
            }
        }

        private void SetTabTitle()
            => TabTitle = $"{SelectedItem.Model.LastName}, {SelectedItem.Model.FirstName}";

        private void OnAddAuthorPictureExecute()
        {
            //SelectedItem.MugshotPath = FileExplorerService.BrowsePicture() ?? SelectedItem.MugshotPath;
            var temp = SelectedItem.MugshotPath;
            SelectedItem.MugshotPath = FileExplorerService.BrowsePicture() ?? SelectedItem.MugshotPath;
            if (!string.IsNullOrEmpty(SelectedItem.MugshotPath)
                && SelectedItem.MugshotPath != temp)
            {
                FileExplorerService.CreateThumbnail(SelectedItem.MugshotPath);
            }
        }

        protected override bool SaveItemCanExecute()
        {
            return (!SelectedItem.HasErrors) && (HasChanges || SelectedItem.Id == default || NationalityIsDirty);
        }

        protected override async void SaveItemExecute()
        {
            if (_nationalityIsDirty)
            {
                var currentNationality =
                    await ((AuthorRepository) DomainService.Repository).GetNationalityAsync(SelectedNationality.Id);
                SelectedItem.Model.SetNationality(currentNationality);
            }
            base.SaveItemExecute();
        }

        public override async Task LoadAsync(Guid id)
        {
            try
            {
                Author author = null;

                if (id != default)
                {
                    author = await ((AuthorRepository) DomainService.Repository).LoadAsync(id);
                }
                else
                {
                    author = Author.NewAuthor;
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
                    this.SwitchEditableStateExecute();
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

            await InitializeNationalityCollection();

            async Task InitializeNationalityCollection()
            {
                if (!Nationalities.Any())
                {
                    Nationalities.Clear();

                    foreach (var item in await GetNationalityList())
                    {
                        Nationalities.Add(item);
                    }

                    if (SelectedItem.Model.Nationality != null)
                        SelectedNationality = Nationalities.FirstOrDefault(n => n.Id == SelectedItem.Model.Nationality.Id);
                }
            }
        }

        protected override string CreateChangeMessage(DatabaseOperation operation)
            => $"{operation}: {SelectedItem.LastName}, {SelectedItem.FirstName}.";

        private async Task<IEnumerable<LookupItem>> GetNationalityList()
            => await ((AuthorService)DomainService).NationalityLookupDataService
                                                     .GetNationalityLookupAsync(nameof(NationalityDetailViewModel));

        private void OnNationalitySelectionChangedExecute()
        {
            if (SelectedNationality is not null && Nationalities.Any())
            {
                NationalityIsDirty = SelectedItem.Model.Nationality.Id != SelectedNationality.Id;
            }
        }


        private void OnAddNewNationalityExecute()
        {
            EventAggregator.GetEvent<OpenDetailViewEvent>()
                           .Publish(new OpenDetailViewEventArgs
                           {
                               Id = new Guid(),
                               ViewModelName = nameof(NationalityDetailViewModel)
                           });
        }

        public override AuthorWrapper CreateWrapper(Author entity)
        {
            var wrapper = new AuthorWrapper(entity);
            return wrapper;
        }
    }
}
