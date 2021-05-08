using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.DA.Conditions;
using BookOrganizer2.Domain.Shared;
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
using System.Windows.Threading;
using BookOrganizer2.Domain.BookProfile.FormatProfile;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class BooksViewModel : BaseViewModel
    {
        private readonly IBookLookupDataService _bookLookupDataService;
        private bool _allGenresIsSelected;

        public BooksViewModel(IEventAggregator eventAggregator,
            IBookLookupDataService bookLookupDataService,
            ILogger logger,
            IDialogService dialogService)
            : base(eventAggregator, logger, dialogService)
        {
            _bookLookupDataService = bookLookupDataService
                                           ?? throw new ArgumentNullException(nameof(bookLookupDataService));

            GenreFilterExecutedCommand = new DelegateCommand(OnGenreFilterExecuted);
            AllGenresSelectionChangedCommand = new DelegateCommand(OnAllGenresSelectionChangedExecuted);

            Filters = GetFilters();
            ActiveFilter = Filters.First();

            Init().Await();

            ViewModelType = nameof(BookDetailViewModel);
        }

        private void OnAllGenresSelectionChangedExecuted()
        {
            //AllGenresIsSelected = !AllGenresIsSelected;
            // TODO: filter
        }

        private void OnGenreFilterExecuted()
        {
            // TODO: Genres => Dictionary<GenreLookupItem, bool>??

            throw new NotImplementedException();
        }

        public ICommand GenreFilterExecutedCommand { get; }
        public ICommand AllGenresSelectionChangedCommand { get; }
        public ObservableCollection<GenreLookupItem> Genres { get; set; }

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
            List<GenreLookupItem> genreFilter = null;
            List<FormatLookupItem> formatFilter = null;

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
