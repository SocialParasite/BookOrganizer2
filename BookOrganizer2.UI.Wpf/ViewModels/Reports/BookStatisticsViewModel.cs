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
            this._viewModelCreator = viewModelCreator ?? throw new ArgumentNullException(nameof(viewModelCreator));

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
            Reports = new ObservableCollection<IReport>();
            Reports.Add(_viewModelCreator[nameof(AnnualBookStatisticsReportViewModel)]);
            Reports.Add(_viewModelCreator[nameof(AnnualBookStatisticsInRangeReportViewModel)]);
            Reports.Add(_viewModelCreator[nameof(MonthlyReadsReportViewModel)]);
            SelectedReport = Reports[0];
        }
    }
}
