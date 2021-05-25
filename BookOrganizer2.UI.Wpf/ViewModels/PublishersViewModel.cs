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
    public class PublishersViewModel : BaseViewModel
    {
        private readonly IPublisherLookupDataService _publisherLookupDataService;

        public PublishersViewModel(IEventAggregator eventAggregator,
            IPublisherLookupDataService publisherLookupDataService,
            ILogger logger,
            IDialogService dialogService)
            : base(eventAggregator, logger, dialogService)
        {
            _publisherLookupDataService = publisherLookupDataService 
                                          ?? throw new ArgumentNullException(nameof(publisherLookupDataService));

            Filters = GetFilters();
            ActiveFilter = Filters.First();

            Init().Await();

            ViewModelType = nameof(PublisherDetailViewModel);
        }

        public override string InfoText { get; set; } = "Publishers shown";

        private Task Init()
            => Task.Run(InitializeRepositoryAsync);

        public override async Task InitializeRepositoryAsync()
        {
            try
            {
                Items = await _publisherLookupDataService.GetPublisherLookupAsync(nameof(PublisherDetailViewModel));

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

            Items = await _publisherLookupDataService
                .GetFilteredPublisherLookupAsync(nameof(PublisherDetailViewModel), condition)
                .ConfigureAwait(false);

            UpdateEntityCollection();
            SearchString = "";
        }

        private void UpdateEntityCollection()
        {
            EntityCollection = Items.OrderBy(p => p.DisplayMember).ToList();

            NumberOfItems = EntityCollection.Count;
        }

        private static PublisherFilterCondition MapActiveFilterToFilterCondition(string filter)
        {
            return filter switch
            {
                "No filter" => PublisherFilterCondition.NoFilter,
                "Publishers without description" => PublisherFilterCondition.NoDescription,
                "Publishers without books" => PublisherFilterCondition.NoBooks,
                "Publishers without logo" => PublisherFilterCondition.NoLogoPicture,
                _ => throw new ArgumentOutOfRangeException(nameof(filter), "Invalid filter condition")
            };
        }

        private static IEnumerable<string> GetFilters()
        {
            yield return "No filter";
            yield return "Publishers without description";
            yield return "Publishers without books";
            yield return "Publishers without logo";
        }
    }
}

