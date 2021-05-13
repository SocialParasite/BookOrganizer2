using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.DA.Conditions;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using BookOrganizer2.UI.Wpf.Extensions;
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
    public class BooksViewModel : BaseViewModel
    {
        private readonly IBookLookupDataService _bookLookupDataService;
        private bool _allFormatsIsSelected;
        private bool _allGenresIsSelected;
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

            Filters = GetFilters();
            ActiveFilter = Filters.First();

            Init().Await();

            ViewModelType = nameof(BookDetailViewModel);
        }

        public ICommand GenreFilterExecutedCommand { get; }
        public ICommand FormatFilterExecutedCommand { get; }
        public ICommand AllFormatsSelectionChangedCommand { get; }
        public ICommand AllGenresSelectionChangedCommand { get; }

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
                Genres = (await _bookLookupDataService.GetGenresAsync()).FromListToObservableCollection();
                Formats = (await _bookLookupDataService.GetFormatsAsync()).FromListToObservableCollection();
            }
            catch (Exception ex)
            {
                var dialog = new NotificationViewModel("Exception", ex.Message);
                DialogService.OpenDialog(dialog);

                Logger.Error("Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.Message, ex.StackTrace);
            }

            ActiveFilter = Filters.First();
        }

        protected override async Task FilterCollection(bool resetFilters = false)
        {
            if (resetFilters)
            {
                await InitializeRepositoryAsync();
            }

            var condition = MapActiveFilterToFilterCondition(ActiveFilter);
            
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
                .GetFilteredBookLookupAsync(nameof(BookDetailViewModel), condition, genreFilter, formatFilter)
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

        private void OnAllGenresSelectionChangedExecuted()
        {
            //foreach (var genre in Genres)
            //{
            //    genre.IsSelected = true;
            //}

            FilterCollection().Await();
        }

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
        private static FilterCondition MapActiveFilterToFilterCondition(string filter)
        {
            return filter switch
            {
                "No filter" => FilterCondition.NoFilter,
                "Books without description" => FilterCondition.NoDescription,
                "Books with placeholder picture as cover" => FilterCondition.PlaceholderCover,
                "Books without author" => FilterCondition.NoAuthors,
                "Books not read" => FilterCondition.NotRead,
                _ => throw new ArgumentOutOfRangeException(nameof(filter), "Invalid filter condition")
            };
        }

        private static IEnumerable<string> GetFilters()
        {
            yield return "No filter";
            yield return "Books without description";
            yield return "Books with placeholder picture as cover";
            yield return "Books without author";
            yield return "Books not read";
        }
    }
}
