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

                EntityCollection = Items
                    .OrderBy(b => b.DisplayMember
                                      .StartsWith("A ", StringComparison.OrdinalIgnoreCase)
                                  || b.DisplayMember.StartsWith("The ", StringComparison.OrdinalIgnoreCase)
                        ? b.DisplayMember.Substring(b.DisplayMember.IndexOf(" ", StringComparison.Ordinal) + 1)
                        : b.DisplayMember)
                    .ToList();
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
