using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.DA.Conditions;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using BookOrganizer2.UI.Wpf.ViewModels.DetailViewModels;
using Prism.Events;
using Serilog;

namespace BookOrganizer2.UI.Wpf.ViewModels.ListViewModels
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

            MaintenanceFilters = GetMaintenanceFilters();
            ActiveMaintenanceFilter = MaintenanceFilters.First();

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

            ActiveMaintenanceFilter = MaintenanceFilters.First();
        }

        protected override async Task FilterCollection(bool resetFilters = false)
        {
            if (resetFilters)
            {
                await InitializeRepositoryAsync();
            }

            var condition = MapActiveFilterToFilterCondition(ActiveMaintenanceFilter);

            Items = await _authorLookupDataService
                .GetAuthorLookupAsync(nameof(AuthorDetailViewModel), condition)
                .ConfigureAwait(false);

            UpdateEntityCollection();
            SearchString = "";
        }

        private void UpdateEntityCollection()
        {
            EntityCollection = Items.OrderBy(p => p.DisplayMember).ToList();

            NumberOfItems = EntityCollection.Count;
        }

        private static AuthorMaintenanceFilterCondition MapActiveFilterToFilterCondition(string filter)
        {
            return filter switch
            {
                "No filter" => AuthorMaintenanceFilterCondition.NoFilter,
                "Authors without biography" => AuthorMaintenanceFilterCondition.NoBio,
                "Authors without books" => AuthorMaintenanceFilterCondition.NoBooks,
                "Authors without date of birth" => AuthorMaintenanceFilterCondition.NoDateOfBirth,
                "Authors without nationality" => AuthorMaintenanceFilterCondition.NoNationality,
                "Authors with placeholder picture as mugshot" => AuthorMaintenanceFilterCondition.NoMugshot,
                _ => throw new ArgumentOutOfRangeException(nameof(filter), "Invalid filter condition")
            };
        }

        private static IEnumerable<string> GetMaintenanceFilters()
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
