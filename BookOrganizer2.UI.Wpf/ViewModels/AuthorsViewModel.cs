using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.DA;
using Prism.Events;
using Serilog;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class AuthorsViewModel : BaseViewModel<Author, AuthorId>
    {
        private readonly IAuthorLookupDataService _authorLookupDataService;

        public AuthorsViewModel(IEventAggregator eventAggregator,
            IAuthorLookupDataService authorLookupDataService,
            ILogger logger
            /*IDialogService dialogService*/)
            : base(eventAggregator, logger/*, dialogService*/)
        {
            this._authorLookupDataService = authorLookupDataService
                                           ?? throw new ArgumentNullException(nameof(authorLookupDataService));

            Init();

            ViewModelType = nameof(AuthorDetailViewModel);
        }

        private Task Init()
            => InitializeRepositoryAsync();

        public override async Task InitializeRepositoryAsync()
        {
            try
            {
                items = await _authorLookupDataService.GetAuthorLookupAsync(nameof(AuthorDetailViewModel));

                EntityCollection = items.OrderBy(p => p.DisplayMember).ToList();
            }
            catch (Exception ex)
            {
                //var dialog = new NotificationViewModel("Exception", ex.Message);
                //dialogService.OpenDialog(dialog);

                //logger.Error("Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.Message, ex.StackTrace);
            }
        }
    }
}
