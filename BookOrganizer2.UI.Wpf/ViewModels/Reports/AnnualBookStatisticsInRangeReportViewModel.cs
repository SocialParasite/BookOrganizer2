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
    public class AnnualBookStatisticsInRangeReportViewModel : ViewModelBase, IReport
    {
        private readonly ILogger _logger;
        private readonly IBookStatisticsYearRangeLookupDataService _lookupService;
        private readonly IDialogService _dialogService;

        private List<AnnualBookStatisticsInRangeReport> _reportData;
        private int _selectedBeginYear;
        private int _selectedEndYear;

        public AnnualBookStatisticsInRangeReportViewModel(IBookStatisticsYearRangeLookupDataService lookupService,
                                                          ILogger logger,
                                                          IDialogService dialogService)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            YearSelectionChangedCommand = new DelegateCommand(OnYearSelectionChangedExecute);

            Init();
        }

        public ICommand YearSelectionChangedCommand { get; set; }
        public string ReportName => "Books read during period report";
        public IEnumerable<int> YearsList { get; set; }
        public List<AnnualBookStatisticsInRangeReport> ReportData
        {
            get => _reportData;
            set
            {
                _reportData = value;
                OnPropertyChanged();
            }
        }

        public int SelectedBeginYear
        {
            get => _selectedBeginYear;
            set { _selectedBeginYear = value; OnPropertyChanged(); }
        }

        public int SelectedEndYear
        {
            get => _selectedEndYear;
            set { _selectedEndYear = value; OnPropertyChanged(); }
        }

        private Task Init(int? beginYear = null, int? endYear = null)
            => InitializeReportAsync(beginYear, endYear);

        private async Task InitializeReportAsync(int? beginYear = null, int? endYear = null)
        {
            if (SelectedBeginYear == default)
            {
                SelectedBeginYear = beginYear ?? DateTime.Now.Year - 10;
            }
            if (SelectedEndYear == default)
            {
                SelectedEndYear = endYear ?? DateTime.Now.Year;
            }
            if (SelectedEndYear < SelectedBeginYear)
            {
                SelectedEndYear = SelectedBeginYear;
            }

            YearsList = PopulateYearsMenu();

            try
            {
                var temp = await _lookupService.GetAnnualBookStatisticsInRangeReportAsync(SelectedBeginYear, SelectedEndYear);
                ReportData = new List<AnnualBookStatisticsInRangeReport>(temp);
            }
            catch (SqlNullValueException ex)
            {
                var details = $"No statistics found for year range {SelectedBeginYear} - {SelectedEndYear}";
                var dialog = new NotificationViewModel("Error!", details);

                _dialogService.OpenDialog(dialog);

                _logger.Error("Exception: {Exception} Details: {Details} Message: {Message}\n\n Stack trace: {StackTrace}\n\n",
                    ex.GetType().Name, details, ex.Message, ex.StackTrace);
            }
            catch (Exception ex)
            {
                var dialog = new NotificationViewModel("Error!", ex.Message);
                _dialogService.OpenDialog(dialog);

                _logger.Error("Exception: {Exception} Message: {Message}\n\n Stack trace: {StackTrace}\n\n",
                    ex.GetType().Name, ex.Message, ex.StackTrace);
            }
        }

        private void OnYearSelectionChangedExecute()
            => Init(SelectedBeginYear, SelectedEndYear);

        private IEnumerable<int> PopulateYearsMenu()
        {
            for (int year = DateTime.Today.Year; year > 0; year--)
                yield return year;
        }
    }
}
