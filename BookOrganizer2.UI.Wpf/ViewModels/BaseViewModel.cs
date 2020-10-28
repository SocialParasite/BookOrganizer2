using BookOrganizer2.Domain.Shared;
using BookOrganizer2.UI.Wpf.Events;
using BookOrganizer2.UI.Wpf.Extensions;
using BookOrganizer2.UI.Wpf.Interfaces;
using Prism.Commands;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public abstract class BaseViewModel<T, TId> : ViewModelBase, IItemLists
                where T : class, IIdentifiable<TId>
                where TId : ValueObject
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
        }

        protected IEnumerable<LookupItem> Items;
        public ICommand AddNewItemCommand { get; }
        public ICommand ItemNameLabelMouseLeftButtonUpCommand { get; }

        public string ViewModelType { get; set; }

        public List<LookupItem> EntityCollection
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

        private void UpdateFilteredEntityCollection()
        {
            FilteredEntityCollection?.Clear();
            FilteredEntityCollection = EntityCollection?.Where(w => w.DisplayMember
                                                       .IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) != -1)
                                                       .ToList();
        }

        public abstract Task InitializeRepositoryAsync();

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
            => (item.Id != Guid.Empty);

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
