using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.DA.Conditions;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using BookOrganizer2.UI.Wpf.ViewModels.DetailViewModels;
using JetBrains.Annotations;
using Prism.Events;
using Serilog;

namespace BookOrganizer2.UI.Wpf.ViewModels.ListViewModels
{
    public class SeriesViewModel : BaseViewModel
    {
        private readonly ISeriesLookupDataService _seriesLookupDataService;

        public SeriesViewModel(IEventAggregator eventAggregator,
            ISeriesLookupDataService seriesLookupDataService,
            ILogger logger,
            IDialogService dialogService)
            : base(eventAggregator, logger, dialogService)
        {
            _seriesLookupDataService = seriesLookupDataService
                                          ?? throw new ArgumentNullException(nameof(seriesLookupDataService));


            MaintenanceFilters = GetMaintenanceFilters();
            Filters = GetFilters();
            ActiveMaintenanceFilter = MaintenanceFilters.First();

            Init().Await();

            ViewModelType = nameof(SeriesDetailViewModel);
        }

        public override string InfoText { get; set; } = "Series shown";
        [UsedImplicitly] public IEnumerable<string> Filters { get; set; }

        private string _activeFilter;
        [UsedImplicitly]
        public string ActiveFilter
        {
            get => _activeFilter;
            set { _activeFilter = value; OnPropertyChanged(); }
        }

        private Task Init()
            => Task.Run(InitializeRepositoryAsync);

        public override async Task InitializeRepositoryAsync()
        {
            try
            {
                Items = await _seriesLookupDataService.GetSeriesLookupAsync(nameof(SeriesDetailViewModel));

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
            ActiveFilter = Filters.First();
        }

        protected override async Task FilterCollection(bool resetFilters = false)
        {
            if (resetFilters)
            {
                await InitializeRepositoryAsync();
            }

            var condition = MapActiveMaintenanceFilterToMaintenanceFilterCondition(ActiveMaintenanceFilter);
            var condition2 = MapActiveFilterToFilterCondition(ActiveFilter);

            Items = await _seriesLookupDataService
                .GetSeriesLookupAsync(nameof(SeriesDetailViewModel), condition, condition2)
                .ConfigureAwait(false);

            UpdateEntityCollection();
            SearchString = "";
        }

        private void UpdateEntityCollection()
        {
            EntityCollection = Items.OrderBy(p => p.DisplayMember).ToList();

            NumberOfItems = EntityCollection.Count;
        }

        private static SeriesMaintenanceFilterCondition MapActiveMaintenanceFilterToMaintenanceFilterCondition(string filter)
        {
            return filter switch
            {
                "No filter" => SeriesMaintenanceFilterCondition.NoFilter,
                "Series without description" => SeriesMaintenanceFilterCondition.NoDescription,
                "Series without books" => SeriesMaintenanceFilterCondition.NoBooks,
                "Series without picture" => SeriesMaintenanceFilterCondition.NoPicture,
                _ => throw new ArgumentOutOfRangeException(nameof(filter), "Invalid filter condition")
            };
        }

        private static IEnumerable<string> GetMaintenanceFilters()
        {
            yield return "No filter";
            yield return "Series without description";
            yield return "Series without books";
            yield return "Series without picture";
        }

        private SeriesFilterCondition MapActiveFilterToFilterCondition(string activeFilter)
        {
            return activeFilter switch
            {
                "No filter" => SeriesFilterCondition.NoFilter,
                "Series not started" => SeriesFilterCondition.NotStarted,
                "Series partly read" => SeriesFilterCondition.PartlyRead,
                "Series not all books owned" => SeriesFilterCondition.NotFullyOwned,
                _ => throw new ArgumentOutOfRangeException(nameof(activeFilter), "Invalid filter condition")
            };
        }

        private static IEnumerable<string> GetFilters()
        {
            yield return "No filter";
            yield return "Series not started";
            yield return "Series partly read";
            yield return "Series not all books owned";
        }
    }
}
