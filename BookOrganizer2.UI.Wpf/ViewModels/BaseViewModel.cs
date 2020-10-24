using Prism.Commands;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Shared;
using BookOrganizer2.UI.Wpf.Events;
using BookOrganizer2.UI.Wpf.Extensions;
using BookOrganizer2.UI.Wpf.Interfaces;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public abstract class BaseViewModel<T, TId> : ViewModelBase, IItemLists
                where T : class, IIdentifiable<TId>
                where TId : ValueObject
    {
        private List<LookupItem> entityCollection;
        public readonly IEventAggregator eventAggregator;
        protected readonly ILogger logger;
        //protected readonly IDialogService dialogService;

        protected BaseViewModel(IEventAggregator eventAggregator,
                             ILogger logger
                             /*IDialogService dialogService*/)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            //AddNewItemCommand = new DelegateCommand<string>(OnAddNewItemExecute);

            ItemNameLabelMouseLeftButtonUpCommand =
                new DelegateCommand<LookupItem>(OnItemNameLabelMouseLeftButtonUpExecute,
                                           OnItemNameLabelMouseLeftButtonUpCanExecute);
        }

        public IEnumerable<LookupItem> items;
        public IRepository<T, TId> repository;
        public ICommand AddNewItemCommand { get; }
        public ICommand ItemNameLabelMouseLeftButtonUpCommand { get; }

        public string ViewModelType { get; set; }

        public List<LookupItem> EntityCollection
        {
            get => entityCollection;
            set
            {
                entityCollection = value;
                FilteredEntityCollection = entityCollection.FromListToList();
                OnPropertyChanged();
            }
        }

        private List<LookupItem> filteredEntityCollection;

        public List<LookupItem> FilteredEntityCollection
        {
            get => filteredEntityCollection;
            set
            {
                filteredEntityCollection = value;
                OnPropertyChanged();
            }
        }

        private string searchString;

        public string SearchString
        {
            get => searchString;
            set
            {
                searchString = value;
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

        //private void OnAddNewItemExecute(string itemType)
        //{
        //    eventAggregator.GetEvent<OpenDetailViewEvent>()
        //               .Publish(new OpenDetailViewEventArgs
        //               {
        //                   Id = new Guid(),
        //                   ViewModelName = itemType
        //               });
        //}

        private bool OnItemNameLabelMouseLeftButtonUpCanExecute(LookupItem item)
            => (item.Id != Guid.Empty);

        private void OnItemNameLabelMouseLeftButtonUpExecute(LookupItem item)
        {
            eventAggregator.GetEvent<OpenDetailViewEvent>()
                           .Publish(new OpenDetailViewEventArgs
                           {
                               Id = item.Id,
                               ViewModelName = item.ViewModelName
                           });
        }
    }
}
