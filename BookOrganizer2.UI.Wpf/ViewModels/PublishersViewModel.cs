using BookOrganizer2.Domain.DA;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using Prism.Events;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

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

            Init().Await();

            ViewModelType = nameof(PublisherDetailViewModel);
        }

        private Task Init()
            => Task.Run(InitializeRepositoryAsync);

        public override async Task InitializeRepositoryAsync()
        {
            try
            {
                Items = await _publisherLookupDataService.GetPublisherLookupAsync(nameof(PublisherDetailViewModel));

                EntityCollection = Items.OrderBy(p => p.DisplayMember).ToList();
            }
            catch (Exception ex)
            {
                var dialog = new NotificationViewModel("Exception", ex.Message);
                DialogService.OpenDialog(dialog);

                Logger.Error("Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.Message, ex.StackTrace);
            }
        }
    }
}

