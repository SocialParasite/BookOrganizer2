using System;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA.Reports;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using Serilog;
using Serilog.Core;

namespace BookOrganizer2.UI.Wpf.ViewModels.Reports
{
    public class MaintenanceReportViewModel : ViewModelBase, IReport
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;
        private readonly IMaintenanceReportLookupDataService _lookupDataService;
        private MaintenanceReportItems _items;
        private string _reportLabel;
        public string ReportName => "Maintenance Report";
        public string ReportLabel
        {
            get { return _reportLabel; }
            set { _reportLabel = value; OnPropertyChanged(); }
        }

        public MaintenanceReportItems Items
        {
            get => _items;
            set { _items = value; OnPropertyChanged(); }
        }

        public MaintenanceReportViewModel(ILogger logger, IDialogService dialogService, IMaintenanceReportLookupDataService lookupDataService)
        {
            _logger = logger;
            _dialogService = dialogService;
            _lookupDataService = lookupDataService;

            Init().Await();
        }
        // inject several VM's here?
        private Task Init()
            => Task.Run(InitializeRepositoryAsync);

        public async Task InitializeRepositoryAsync()
        {
            try
            {
                Items = await _lookupDataService.GetMaintenanceData();
                ReportLabel = $"Books missing description: {Items.BooksWithoutDescriptionCount} / {Items.BookCount}";
            }
            catch (Exception ex)
            {
                var dialog = new NotificationViewModel("Exception", ex.Message);
                _dialogService.OpenDialog(dialog);

                _logger.Error(ex, "Message: {Message}\n\n Stack trace: {StackTrace}\n\n", 
                    ex.Message, ex.StackTrace);
            }
        }
    }

    //maintenance report

    //- index status(fragmentation)

    //- books not read / read ratio(monthly fluctutation)
    //- duplicate check(authors, publishers, books, series)

    //if book has a read date, the isRead checkbox should be disabled

    //Audiobooks : Books ???
    //- length
    //- reader
    //- store / service? if owned
    //- ppv?
}