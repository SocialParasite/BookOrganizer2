using System;
using System.Linq;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using Prism.Events;
using Serilog;

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

            Init().Await();

            ViewModelType = nameof(SeriesDetailViewModel);
        }

        private async Task Init()
            => await Task.Run(InitializeRepositoryAsync);

        public override async Task InitializeRepositoryAsync()
        {
            try
            {
                Items = await _seriesLookupDataService.GetSeriesLookupAsync(nameof(SeriesDetailViewModel));

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
