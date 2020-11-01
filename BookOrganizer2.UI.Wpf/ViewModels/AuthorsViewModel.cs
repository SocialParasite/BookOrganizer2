using BookOrganizer2.Domain.DA;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using Prism.Events;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
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

            Init().Await();
        }


        private async Task Init()
            => await Task.Run(InitializeRepositoryAsync);

        public override async Task InitializeRepositoryAsync()
        {
            try
            {
                Items = await _authorLookupDataService.GetAuthorLookupAsync(nameof(AuthorDetailViewModel));

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
