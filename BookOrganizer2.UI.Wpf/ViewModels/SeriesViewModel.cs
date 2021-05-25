using BookOrganizer2.Domain.DA;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA.Conditions;

namespace BookOrganizer2.UI.Wpf.ViewModels
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


            Filters = GetFilters();
            ActiveFilter = Filters.First();

            Init().Await();

            ViewModelType = nameof(SeriesDetailViewModel);
        }

        public override string InfoText { get; set; } = "Series shown";

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

            ActiveFilter = Filters.First();
        }

        protected override async Task FilterCollection(bool resetFilters = false)
        {
            if (resetFilters)
            {
                await InitializeRepositoryAsync();
            }

            var condition = MapActiveFilterToFilterCondition(ActiveFilter);

            Items = await _seriesLookupDataService
                .GetFilteredSeriesLookupAsync(nameof(SeriesDetailViewModel), condition)
                .ConfigureAwait(false);

            UpdateEntityCollection();
            SearchString = "";
        }

        private void UpdateEntityCollection()
        {
            EntityCollection = Items.OrderBy(p => p.DisplayMember).ToList();

            NumberOfItems = EntityCollection.Count;
        }

        private static SeriesFilterCondition MapActiveFilterToFilterCondition(string filter)
        {
            return filter switch
            {
                "No filter" => SeriesFilterCondition.NoFilter,
                "Series without description" => SeriesFilterCondition.NoDescription,
                "Series without books" => SeriesFilterCondition.NoBooks,
                "Series without picture" => SeriesFilterCondition.NoPicture,
                _ => throw new ArgumentOutOfRangeException(nameof(filter), "Invalid filter condition")
            };
        }

        private static IEnumerable<string> GetFilters()
        {
            yield return "No filter";
            yield return "Series without description";
            yield return "Series without books";
            yield return "Series without picture";
        }
    }
}
