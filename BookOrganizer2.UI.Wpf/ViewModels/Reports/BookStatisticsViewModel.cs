using Autofac.Features.Indexed;
using BookOrganizer2.UI.Wpf.Interfaces;
using System;
using System.Collections.ObjectModel;

namespace BookOrganizer2.UI.Wpf.ViewModels.Reports
{
    public class BookStatisticsViewModel : ViewModelBase, ISelectedViewModel
    {
        private IReport _selectedReport;
        private readonly IIndex<string, IReport> _viewModelCreator;

        public BookStatisticsViewModel(IIndex<string, IReport> viewModelCreator)
        {
            _viewModelCreator = viewModelCreator ?? throw new ArgumentNullException(nameof(viewModelCreator));

            InitializeView();
        }

        public IReport SelectedReport
        {
            get => _selectedReport;
            set { _selectedReport = value; OnPropertyChanged(); }
        }

        public ObservableCollection<IReport> Reports { get; set; }

        private void InitializeView()
        {
            Reports = new ObservableCollection<IReport>
            {
                _viewModelCreator[nameof(AnnualBookStatisticsReportViewModel)],
                _viewModelCreator[nameof(AnnualBookStatisticsInRangeReportViewModel)],
                _viewModelCreator[nameof(MonthlyReadsReportViewModel)],
                _viewModelCreator[nameof(MaintenanceReportViewModel)]
            };
            SelectedReport = Reports[0];
        }
    }
}
