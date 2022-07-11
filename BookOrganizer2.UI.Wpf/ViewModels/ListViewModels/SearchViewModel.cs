﻿using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Shared;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using BookOrganizer2.UI.Wpf.Interfaces;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using BookOrganizer2.UI.Wpf.Events;
using BookOrganizer2.UI.Wpf.Extensions;
using JetBrains.Annotations;
using Prism.Commands;

namespace BookOrganizer2.UI.Wpf.ViewModels.ListViewModels
{
    public class SearchViewModel :  ViewModelBase, IItemLists
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISearchLookupDataService _searchLookupDataService;
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;
        private ObservableCollection<SearchResult> _items;

        public ObservableCollection<SearchResult> Items
        {
            get => _items;
            set { _items = value; OnPropertyChanged(); }
        }

        public IList<SearchResult> SearchResults { get; set; }

        public SearchViewModel(IEventAggregator eventAggregator,
            ISearchLookupDataService searchLookupDataService,
            ILogger logger,
            IDialogService dialogService)
        {
            _eventAggregator = eventAggregator;
            _searchLookupDataService = searchLookupDataService
                                       ?? throw new ArgumentNullException(nameof(searchLookupDataService));
            _logger = logger;
            _dialogService = dialogService;

            ItemNameLabelMouseLeftButtonUpCommand =
                new DelegateCommand<SearchResult>(OnItemNameLabelMouseLeftButtonUpExecute,
                    OnItemNameLabelMouseLeftButtonUpCanExecute);

            Init().Await();
        }

        public string SearchTerm { get; set; }
        [UsedImplicitly] public ICommand ItemNameLabelMouseLeftButtonUpCommand { get; }

        private Task Init()
            => Task.Run(InitializeRepositoryAsync);

        public async Task InitializeRepositoryAsync()
        {
            try
            {
                SearchResults = await _searchLookupDataService.Search(SearchTerm);
                
                await UpdateItemCollection();
            }
            catch (Exception ex)
            {
                var dialog = new NotificationViewModel("Exception", ex.Message);
                _dialogService.OpenDialog(dialog);

                _logger.Error("Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.Message, ex.StackTrace);
            }
        }

        private async Task UpdateItemCollection() 
            => Items = (await _searchLookupDataService.Search(SearchTerm)).ToObservableCollection();

        private bool OnItemNameLabelMouseLeftButtonUpCanExecute(SearchResult item)
            => item.Id != Guid.Empty;

        private void OnItemNameLabelMouseLeftButtonUpExecute(SearchResult item)
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                .Publish(new OpenDetailViewEventArgs
                {
                    Id = item.Id,
                    ViewModelName = item.ViewModelName
                });
        }
    }
}
