using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.DA.Conditions;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using BookOrganizer2.UI.Wpf.Extensions;
using BookOrganizer2.UI.Wpf.ViewModels.DetailViewModels;
using Prism.Commands;
using Prism.Events;
using Serilog;

namespace BookOrganizer2.UI.Wpf.ViewModels.ListViewModels
{
    public class BooksViewModel : BaseViewModel
    {
        private readonly IBookLookupDataService _bookLookupDataService;
        private bool _allFormatsIsSelected;
        private bool _allGenresIsSelected;
        private bool _showOnlyBooksNotRead;
        private bool _showOnlyNotOwnedBooks;

        private ObservableCollection<GenreLookupItem> _genres;
        private ObservableCollection<FormatLookupItem> _formats;

        public BooksViewModel(IEventAggregator eventAggregator,
            IBookLookupDataService bookLookupDataService,
            ILogger logger,
            IDialogService dialogService)
            : base(eventAggregator, logger, dialogService)
        {
            _bookLookupDataService = bookLookupDataService
                                           ?? throw new ArgumentNullException(nameof(bookLookupDataService));

            FormatFilterExecutedCommand = new DelegateCommand<Guid?>(OnFormatFilterExecuted);
            GenreFilterExecutedCommand = new DelegateCommand<Guid?>(OnGenreFilterExecuted);
            AllGenresSelectionChangedCommand = new DelegateCommand(OnAllGenresSelectionChangedExecuted);
            AllFormatsSelectionChangedCommand = new DelegateCommand(OnAllGenresSelectionChangedExecuted);
            ShowOnlyNotReadBooksCommand = new DelegateCommand(OnShowOnlyNotReadBooksExecute);
            ShowOnlyNotOwnedBooksCommand = new DelegateCommand(OnShowOnlyNotOwnedBooksExecute);

            MaintenanceFilters = GetMaintenanceFilters();
            ActiveMaintenanceFilter = MaintenanceFilters.First();

            Init().Await();

            ViewModelType = nameof(BookDetailViewModel);
        }

        public ICommand GenreFilterExecutedCommand { get; }
        public ICommand FormatFilterExecutedCommand { get; }
        public ICommand AllFormatsSelectionChangedCommand { get; }
        public ICommand AllGenresSelectionChangedCommand { get; }
        public ICommand ShowOnlyNotReadBooksCommand { get; }
        public ICommand ShowOnlyNotOwnedBooksCommand { get; }

        public override string InfoText { get; set; } = "Books shown";

        public ObservableCollection<GenreLookupItem> Genres
        {
            get => _genres;
            set
            {
                _genres = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FormatLookupItem> Formats
        {
            get => _formats;
            set
            {
                _formats = value;
                OnPropertyChanged();
            }
        }

        public bool AllFormatsIsSelected
        {
            get => _allFormatsIsSelected;
            set { _allFormatsIsSelected = value; OnPropertyChanged(); }
        }

        public bool AllGenresIsSelected
        {
            get => _allGenresIsSelected;
            set { _allGenresIsSelected = value; OnPropertyChanged(); }
        }

        public bool ShowOnlyBooksNotRead
        {
            get => _showOnlyBooksNotRead;
            set { _showOnlyBooksNotRead = value; OnPropertyChanged(); }
        }

        public bool ShowOnlyNotOwnedBooks
        {
            get => _showOnlyNotOwnedBooks;
            set => _showOnlyNotOwnedBooks = value;
        }

        private Task Init()
            => Task.Run(InitializeRepositoryAsync);

        public override async Task InitializeRepositoryAsync()
        {
            try
            {
                AllGenresIsSelected = true;
                Items = await _bookLookupDataService.GetBookLookupAsync(nameof(BookDetailViewModel));

                AllItemsCount = Items.Count();
                UpdateEntityCollection();
                // TODO: Call book service instead
                Genres = (await _bookLookupDataService.GetGenresAsync()).ToObservableCollection();
                Formats = (await _bookLookupDataService.GetFormatsAsync()).ToObservableCollection();
            }
            catch (Exception ex)
            {
                var dialog = new NotificationViewModel("Exception", ex.Message);
                DialogService.OpenDialog(dialog);

                Logger.Error("Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.Message, ex.StackTrace);
            }

            ActiveMaintenanceFilter = MaintenanceFilters.First();
        }

        protected override async Task FilterCollection(bool resetFilters = false)
        {
            if (resetFilters)
            {
                await InitializeRepositoryAsync();
            }

            var condition = MapActiveFilterToFilterCondition(ActiveMaintenanceFilter);
            
            List<Guid> genreFilter = null;
            List<Guid> formatFilter = null;

            if (!AllGenresIsSelected)
            {
                genreFilter = Genres.Where(g => g.IsSelected).Select(g => g.Id).ToList();
            }
            if (!AllFormatsIsSelected)
            {
                formatFilter = Formats.Where(f => f.IsSelected).Select(f => f.Id).ToList();
            }

            Items = await _bookLookupDataService
                .GetBookLookupAsync(nameof(BookDetailViewModel), condition, ShowOnlyBooksNotRead,
                    ShowOnlyNotOwnedBooks, genreFilter, formatFilter)
                .ConfigureAwait(false);

            UpdateEntityCollection();
            SearchString = "";
        }

        private void UpdateEntityCollection()
        {
            EntityCollection = Items
                .OrderBy(b => b.DisplayMember
                                  .StartsWith("A ", StringComparison.OrdinalIgnoreCase)
                              || b.DisplayMember.StartsWith("The ", StringComparison.OrdinalIgnoreCase)
                    ? b.DisplayMember.Substring(b.DisplayMember.IndexOf(" ", StringComparison.Ordinal) + 1)
                    : b.DisplayMember)
                .ToList();

            NumberOfItems = EntityCollection.Count;
        }

        private void OnAllGenresSelectionChangedExecuted() => FilterCollection().Await();

        private void OnGenreFilterExecuted(Guid? id)
        {
            AllGenresIsSelected = Genres.All(g => g.IsSelected);
            FilterCollection().Await();
        }

        private void OnFormatFilterExecuted(Guid? id)
        {
            AllFormatsIsSelected = Formats.All(f => f.IsSelected);
            FilterCollection().Await();
        }

        private void OnShowOnlyNotReadBooksExecute()
        {
            ShowOnlyBooksNotRead = !ShowOnlyBooksNotRead;
            FilterCollection().Await();
        }

        private void OnShowOnlyNotOwnedBooksExecute()
        {
            ShowOnlyNotOwnedBooks = !ShowOnlyNotOwnedBooks;
            FilterCollection().Await();
        }

        private static BookMaintenanceFilterCondition MapActiveFilterToFilterCondition(string filter)
        {
            return filter switch
            {
                "No filter" => BookMaintenanceFilterCondition.NoFilter,
                "Books without description" => BookMaintenanceFilterCondition.NoDescription,
                "Books with placeholder picture as cover" => BookMaintenanceFilterCondition.PlaceholderCover,
                "Books without author" => BookMaintenanceFilterCondition.NoAuthors,
                "Books without publisher" => BookMaintenanceFilterCondition.NoPublisher,
                _ => throw new ArgumentOutOfRangeException(nameof(filter), "Invalid filter condition")
            };
        }

        private static IEnumerable<string> GetMaintenanceFilters()
        {
            yield return "No filter";
            yield return "Books without description";
            yield return "Books with placeholder picture as cover";
            yield return "Books without author";
            yield return "Books without publisher";
        }
    }
}
