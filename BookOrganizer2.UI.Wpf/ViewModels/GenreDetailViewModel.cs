using BookOrganizer2.Domain.BookProfile.GenreProfile;
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
    public class GenreDetailViewModel : BaseDetailViewModel<Genre, GenreId, GenreWrapper>
    {
        private GenreWrapper _selectedItem;
        private readonly IGenreLookupDataService _genreLookupDataService;

        public GenreDetailViewModel(IEventAggregator eventAggregator,
            ILogger logger,
            IGenreService domainService,
            IGenreLookupDataService genreLookupDataService,
            IDialogService dialogService)
            : base(eventAggregator, logger, domainService, dialogService)
        {
            _genreLookupDataService = genreLookupDataService ?? throw new ArgumentNullException(nameof(genreLookupDataService));
            ChangeEditedGenreCommand = new DelegateCommand<Guid?>(OnChangeEditedGenreExecute);
            SaveItemCommand = new DelegateCommand(SaveItemExecute, base.SaveItemCanExecute)
                .ObservesProperty(() => SelectedItem.Name);

            SelectedItem = CreateWrapper(domainService.CreateItem());

            Genres = new ObservableCollection<LookupItem>();

            UserMode = (!UserMode.Item1, DetailViewState.EditMode, Brushes.LightGray, !UserMode.Item4).ToTuple();
        }

        public ICommand ChangeEditedGenreCommand { get; }

        public sealed override GenreWrapper SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value ?? throw new ArgumentNullException(nameof(SelectedItem));
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LookupItem> Genres { get; }

        protected override string CreateChangeMessage(DatabaseOperation operation) 
            => $"{operation}: {SelectedItem.Name}.";

        public sealed override GenreWrapper CreateWrapper(Genre entity) => new GenreWrapper(entity);

        public override async Task LoadAsync(Guid id)
        {
            try
            {
                Genre genre = null;

                if (id != default)
                {
                    genre = await ((ISimpleDomainService<Genre, GenreId>)DomainService).GetAsync(id) ?? Genre.NewGenre;
                }
                else
                {
                    genre = Genre.NewGenre;
                    IsNewItem = true;
                }

                SelectedItem = CreateWrapper(genre);

                SelectedItem.PropertyChanged += (s, e) =>
                {
                    if (!HasChanges)
                    {
                        HasChanges = DomainService.HasChanges();
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
                    if (!Genres.Any() || HasChanges)
                    {
                        Genres.Clear();

                        foreach (var item in await GetGenreList())
                        {
                            Genres.Add(item);
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

        private async Task<IEnumerable<LookupItem>> GetGenreList()
            => await _genreLookupDataService.GetGenreLookupAsync(nameof(GenreDetailViewModel));

        private async void OnChangeEditedGenreExecute(Guid? genreId)
        {
            if (DomainService.HasChanges())
            {
                var dialog = new OkCancelViewModel("Close the view?", "You have made changes. Changing editable genre will loose all unsaved changes. Are you sure you still want to switch?");
                var result = DialogService.OpenDialog(dialog);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            ((ISimpleDomainService<Genre, GenreId>) DomainService).ResetTracking(SelectedItem.Model);
            HasChanges = DomainService.HasChanges();

            if (genreId is not null) await LoadAsync((Guid) genreId);
        }

        private void NewItemAdded()
        {
            EventAggregator.GetEvent<NewItemEvent>()
                .Publish(new NewItemEventArgs());
        }
    }
}
