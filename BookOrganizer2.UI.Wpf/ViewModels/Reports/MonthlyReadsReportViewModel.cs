using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BookOrganizer2.Domain.DA.Reports;
using BookOrganizer2.Domain.Reports;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using Prism.Commands;
using Serilog;

namespace BookOrganizer2.UI.Wpf.ViewModels.Reports
{
    public class MonthlyReadsReportViewModel : ViewModelBase, IReport
    {
        private readonly ILogger _logger;
        private readonly IMonthlyReadsLookupDataService _lookupService;
        private readonly IDialogService _dialogService;

        private List<MonthlyReadsReport> _reportData;
        private int _selectedYear;
        private Months _selectedMonth;

        public MonthlyReadsReportViewModel(IMonthlyReadsLookupDataService lookupService,
                                           ILogger logger,
                                           IDialogService dialogService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            YearSelectionChangedCommand = new DelegateCommand(OnYearSelectionChangedExecute);
            MonthSelectionChangedCommand = new DelegateCommand(OnMonthSelectionChangedExecute);

            SelectedYear = DateTime.Now.Year;
            SelectedMonth = (Months)DateTime.Now.Month - 1;

            Init(SelectedYear, (int)SelectedMonth + 1);
        }

        public ICommand YearSelectionChangedCommand { get; }
        public ICommand MonthSelectionChangedCommand { get; }

        public string ReportName => "Monthly reads report";
        public IEnumerable<int> YearsList { get; set; }
        public IEnumerable<string> MonthsList { get; set; }

        public List<MonthlyReadsReport> ReportData
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

        public Months SelectedMonth
        {
            get => _selectedMonth;
            set { _selectedMonth = value; OnPropertyChanged(); }
        }

        private IEnumerable<int> PopulateYearsMenu()
        {
            for (int year = DateTime.Today.Year; year > 0; year--)
                yield return year;
        }

        private IEnumerable<string> PopulateMonthsMenu()
        {
            for (int i = 0; i < 12; i++)
            {
                yield return ((Months)i).ToString();
            }
        }

        private Task Init(int? year = null, int? month = null)
            => InitializeRepositoryAsync(year, month);

        private async Task InitializeRepositoryAsync(int? year = null, int? month = null)
        {
            YearsList = PopulateYearsMenu();
            MonthsList = PopulateMonthsMenu();
            month++;

            try
            {
                var monthlyReads = await _lookupService.GetMonthlyReadsReportAsync(year, month);
                ReportData = new List<MonthlyReadsReport>(monthlyReads);
            }
            catch (SqlNullValueException ex)
            {
                var details = $"No statistics found for month {((Months)SelectedMonth + 1).ToString()} of year {SelectedYear}";
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

        private void OnMonthSelectionChangedExecute()
            => Init(SelectedYear, (int)SelectedMonth);
    }

    public enum Months
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }
}
