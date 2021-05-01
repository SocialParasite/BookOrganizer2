using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.DA.Conditions;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class BooksViewModel : BaseViewModel
    {
        private readonly IBookLookupDataService _bookLookupDataService;

        public BooksViewModel(IEventAggregator eventAggregator,
            IBookLookupDataService bookLookupDataService,
            ILogger logger,
            IDialogService dialogService)
            : base(eventAggregator, logger, dialogService)
        {
            _bookLookupDataService = bookLookupDataService
                                           ?? throw new ArgumentNullException(nameof(bookLookupDataService));

            Filters = GetFilters();
            ActiveFilter = Filters.First();

            Init().Await();

            ViewModelType = nameof(BookDetailViewModel);
        }

        private Task Init()
            => Task.Run(InitializeRepositoryAsync);

        public override async Task InitializeRepositoryAsync()
        {
            try
            {
                Items = await _bookLookupDataService.GetBookLookupAsync(nameof(BookDetailViewModel));

                UpdateEntityCollection();
            }
            catch (Exception ex)
            {
                var dialog = new NotificationViewModel("Exception", ex.Message);
                DialogService.OpenDialog(dialog);

                Logger.Error("Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.Message, ex.StackTrace);
            }

            ActiveFilter = Filters.First();
        }

        private FilterCondition MapActiveFilterToFilterCondition(string filter)
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

        private void UpdateEntityCollection()
        {
            EntityCollection = Items
                .OrderBy(b => b.DisplayMember
                                  .StartsWith("A ", StringComparison.OrdinalIgnoreCase)
                              || b.DisplayMember.StartsWith("The ", StringComparison.OrdinalIgnoreCase)
                    ? b.DisplayMember.Substring(b.DisplayMember.IndexOf(" ", StringComparison.Ordinal) + 1)
                    : b.DisplayMember)
                .ToList();

            NumberOfItems = EntityCollection.Count();
        }

        public override async Task FilterCollection(bool resetFilters = false)
        {
            if (resetFilters)
            {
                await InitializeRepositoryAsync();
            }

            var condition = MapActiveFilterToFilterCondition(ActiveFilter);

            Items = await _bookLookupDataService
                .GetFilteredBookLookupAsync(nameof(BookDetailViewModel), condition)
                .ConfigureAwait(false);
            UpdateEntityCollection();
            SearchString = "";
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
