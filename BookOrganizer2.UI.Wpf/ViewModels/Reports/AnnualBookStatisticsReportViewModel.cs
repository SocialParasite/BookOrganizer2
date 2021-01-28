using BookOrganizer2.Domain.DA.Reports;
using BookOrganizer2.Domain.Reports;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using Prism.Commands;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookOrganizer2.UI.Wpf.ViewModels.Reports
{
    public class AnnualBookStatisticsReportViewModel : ViewModelBase, IReport
    {
        private readonly ILogger _logger;
        private readonly IAnnualBookStatisticsLookupDataService _lookupService;
        private readonly IDialogService _dialogService;

        private List<AnnualBookStatisticsReport> _reportData;
        private int _selectedYear;

        public AnnualBookStatisticsReportViewModel(IAnnualBookStatisticsLookupDataService lookupService,
                                                   ILogger logger,
                                                   IDialogService dialogService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            YearSelectionChangedCommand = new DelegateCommand(OnYearSelectionChangedExecute);
            SelectedYear = DateTime.Now.Year;

            Init();
        }

        public ICommand YearSelectionChangedCommand { get; }
        public string ReportName => "Annual books read report";
        public IEnumerable<int> YearsList { get; set; }

        public List<AnnualBookStatisticsReport> ReportData
        {
            get => _reportData;
            set
            {
                _reportData = value;
                OnPropertyChanged();
            }
        }

        public int SelectedYear
        {
            get => _selectedYear;
            set { _selectedYear = value; OnPropertyChanged(); }
        }

        private IEnumerable<int> PopulateYearsMenu()
        {
            for (int year = DateTime.Today.Year; year > 0; year--)
                yield return year;
        }

        private Task Init(int? year = null)
            => InitializeRepositoryAsync(year);

        private async Task InitializeRepositoryAsync(int? year = null)
        {
            YearsList = PopulateYearsMenu();

            try
            {
                var temp = await _lookupService.GetAnnualBookStatisticsReportAsync(year);
                ReportData = new List<AnnualBookStatisticsReport>(temp);
            }
            catch (SqlNullValueException ex)
            {
                var details = $"No statistics found for year {SelectedYear}";
                var dialog = new NotificationViewModel("Error!", details);
                _dialogService.OpenDialog(dialog);
                _logger.Error("Exception: {Exception} Details: {Details} Message: {Message}\n\n Stack trace: {StackTrace}\n\n",
                    ex.GetType().Name, details, ex.Message, ex.StackTrace);
            }
            catch (Exception ex)
            {
                var dialog = new NotificationViewModel("Error!", ex.Message);
                _dialogService.OpenDialog(dialog);
                _logger.Error("Exception: {Exception} Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.GetType().Name, ex.Message, ex.StackTrace);
            }
        }

        private void OnYearSelectionChangedExecute()
            => Init(SelectedYear);
    }

}
