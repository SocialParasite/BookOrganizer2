using BookOrganizer2.Domain.DA;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA.Conditions;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class AuthorsViewModel : BaseViewModel
    {
        private readonly IAuthorLookupDataService _authorLookupDataService;

        public AuthorsViewModel(IEventAggregator eventAggregator,
            IAuthorLookupDataService authorLookupDataService,
            ILogger logger,
            IDialogService dialogService)
            : base(eventAggregator, logger, dialogService)
        {
            _authorLookupDataService = authorLookupDataService
                                           ?? throw new ArgumentNullException(nameof(authorLookupDataService));

            Filters = GetFilters();
            ActiveFilter = Filters.First();

            Init().Await();

            ViewModelType = nameof(AuthorDetailViewModel);
        }

        public override string InfoText { get; set; } = "Authors shown";

        private Task Init()
            => Task.Run(InitializeRepositoryAsync);

        public override async Task InitializeRepositoryAsync()
        {
            try
            {
                Items = await _authorLookupDataService.GetAuthorLookupAsync(nameof(AuthorDetailViewModel));

                AllItemsCount = Items.Count();
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

        protected override async Task FilterCollection(bool resetFilters = false)
        {
            if (resetFilters)
            {
                await InitializeRepositoryAsync();
            }

            var condition = MapActiveFilterToFilterCondition(ActiveFilter);

            Items = await _authorLookupDataService
                .GetFilteredAuthorLookupAsync(nameof(AuthorDetailViewModel), condition)
                .ConfigureAwait(false);

            UpdateEntityCollection();
            SearchString = "";
        }

        private void UpdateEntityCollection()
        {
            EntityCollection = Items.OrderBy(p => p.DisplayMember).ToList();

            NumberOfItems = EntityCollection.Count;
        }

        private static AuthorFilterCondition MapActiveFilterToFilterCondition(string filter)
        {
            return filter switch
            {
                "No filter" => AuthorFilterCondition.NoFilter,
                "Authors without biography" => AuthorFilterCondition.NoBio,
                "Authors without books" => AuthorFilterCondition.NoBooks,
                "Authors without date of birth" => AuthorFilterCondition.NoDateOfBirth,
                "Authors without nationality" => AuthorFilterCondition.NoNationality,
                "Authors with placeholder picture as mugshot" => AuthorFilterCondition.NoMugshot,
                _ => throw new ArgumentOutOfRangeException(nameof(filter), "Invalid filter condition")
            };
        }

        private static IEnumerable<string> GetFilters()
        {
            yield return "No filter";
            yield return "Authors without biography";
            yield return "Authors without books";
            yield return "Authors without date of birth";
            yield return "Authors without nationality";
            yield return "Authors with placeholder picture as mugshot";
        }
    }
}
