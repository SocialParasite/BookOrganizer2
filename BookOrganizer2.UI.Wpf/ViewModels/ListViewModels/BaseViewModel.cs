﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BookOrganizer2.Domain.Shared;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.Wpf.Events;
using BookOrganizer2.UI.Wpf.Extensions;
using BookOrganizer2.UI.Wpf.Interfaces;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Serilog;

namespace BookOrganizer2.UI.Wpf.ViewModels.ListViewModels
{
    public abstract class BaseViewModel : ViewModelBase, IItemLists
    {
        private List<LookupItem> _entityCollection;
        private readonly IEventAggregator _eventAggregator;

        protected readonly ILogger Logger;
        protected readonly IDialogService DialogService;

        protected BaseViewModel(IEventAggregator eventAggregator,
                             ILogger logger,
                             IDialogService dialogService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            AddNewItemCommand = new DelegateCommand<string>(OnAddNewItemExecute);

            ItemNameLabelMouseLeftButtonUpCommand =
                new DelegateCommand<LookupItem>(OnItemNameLabelMouseLeftButtonUpExecute,
                                           OnItemNameLabelMouseLeftButtonUpCanExecute);
            FilterExecutedCommand = new DelegateCommand<bool?>(OnFilterExecute);
        }


        private void OnFilterExecute(bool? resetFilters = false)
        {
            resetFilters ??= false;

            _ = FilterCollection((bool)resetFilters);
        }

        [UsedImplicitly] public string ViewModelType { get; init; }
        protected IEnumerable<LookupItem> Items;
        [UsedImplicitly] public ICommand AddNewItemCommand { get; }
        [UsedImplicitly] public ICommand ItemNameLabelMouseLeftButtonUpCommand { get; }
        [UsedImplicitly] public ICommand FilterExecutedCommand { get; }
        public virtual string InfoText { get; set; }

        protected List<LookupItem> EntityCollection
        {
            get => _entityCollection;
            set
            {
                _entityCollection = value;
                FilteredEntityCollection = _entityCollection.FromListToList();
                OnPropertyChanged();
            }
        }

        private List<LookupItem> _filteredEntityCollection;

        [UsedImplicitly]
        public List<LookupItem> FilteredEntityCollection
        {
            get => _filteredEntityCollection;
            set
            {
                _filteredEntityCollection = value;
                OnPropertyChanged();
            }
        }

        private string _searchString;
        private int _numberOfItems;
        private int _allItemsCount;

        [UsedImplicitly]
        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                OnPropertyChanged();
                UpdateFilteredEntityCollection();
            }
        }

        [UsedImplicitly] public IEnumerable<string> MaintenanceFilters { get; set; }

        private string _activeMaintenanceFilter;
        [UsedImplicitly]
        public string ActiveMaintenanceFilter
        {
            get => _activeMaintenanceFilter;
            set { _activeMaintenanceFilter = value; OnPropertyChanged(); }
        }

        [UsedImplicitly]
        public int NumberOfItems
        {
            get => _numberOfItems;
            set
            {
                _numberOfItems = value;

                _eventAggregator.GetEvent<StatusMessageChangedEvent>()
                    .Publish(new StatusMessageChangedEventArgs
                    {
                        InfoText = InfoText,
                        AllItemsCount = AllItemsCount,
                        NumberOfItems = NumberOfItems
                    });
            }
        }

        [UsedImplicitly]
        public int AllItemsCount
        {
            get => _allItemsCount;
            set
            {
                _allItemsCount = value;
                _eventAggregator.GetEvent<StatusMessageChangedEvent>()
                    .Publish(new StatusMessageChangedEventArgs
                    {
                        InfoText = InfoText,
                        AllItemsCount = AllItemsCount,
                        NumberOfItems = NumberOfItems
                    });
            }
        }

        public abstract Task InitializeRepositoryAsync();
        protected abstract Task FilterCollection(bool resetFilters = false);

        private void UpdateFilteredEntityCollection()
        {
            FilteredEntityCollection?.Clear();
            FilteredEntityCollection = EntityCollection?.Where(w => w.DisplayMember
                                                       .IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) != -1)
                                                       .FromListToList();

            NumberOfItems = FilteredEntityCollection!.Count;
        }

        private void OnAddNewItemExecute(string itemType)
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                       .Publish(new OpenDetailViewEventArgs
                       {
                           Id = new Guid(),
                           ViewModelName = itemType
                       });
        }

        private bool OnItemNameLabelMouseLeftButtonUpCanExecute(LookupItem item)
            => item.Id != Guid.Empty;

        private void OnItemNameLabelMouseLeftButtonUpExecute(LookupItem item)
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
