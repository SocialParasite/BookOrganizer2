﻿using Autofac.Features.Indexed;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using BookOrganizer2.UI.Wpf.Events;
using BookOrganizer2.UI.Wpf.Interfaces;
using BookOrganizer2.UI.Wpf.Services;
using Prism.Commands;
using Prism.Events;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BookOrganizer2.UI.Wpf.ViewModels.DetailViewModels;
using BookOrganizer2.UI.Wpf.ViewModels.ListViewModels;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private IDetailViewModel _selectedDetailViewModel;
        private readonly IIndex<string, IDetailViewModel> _detailViewModelCreator;
        private readonly IIndex<string, ISelectedViewModel> _viewModelCreator;
        private ISelectedViewModel _selectedVm;
        private bool _isViewVisible;
        private bool _isMenuBarVisible;
        private char _pinGlyph;
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public MainViewModel(IEventAggregator eventAggregator,
                              IIndex<string, IDetailViewModel> detailViewModelCreator,
                              IIndex<string, ISelectedViewModel> viewModelCreator,
                              ILogger logger,
                              IDialogService dialogService)
        {
            _detailViewModelCreator = detailViewModelCreator ?? throw new ArgumentNullException(nameof(detailViewModelCreator));
            _viewModelCreator = viewModelCreator ?? throw new ArgumentNullException(nameof(viewModelCreator));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            DetailViewModels = new ObservableCollection<IDetailViewModel>();

            OpenSelectedViewCommand = new DelegateCommand<string>(OnOpenSelectedViewExecute);

            CreateNewItemCommand = new DelegateCommand<Type>(OnCreateNewItemExecute);

            IsMenuBarVisible = true;
            ItemStatusCounter = "";

            OnOpenSelectedViewExecute(nameof(MainPageViewModel));

            SubscribeToEvents();
        }


        private void SubscribeToEvents()
        {
            _eventAggregator.GetEvent<OpenItemViewEvent>()
                .Subscribe(OnOpenSelectedItemView);

            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                .Subscribe(OnOpenDetailViewMatchingSelectedId);

            if (_eventAggregator.GetEvent<OpenItemMatchingSelectedBookIdEvent<Guid>>() is not null)
            {
                _eventAggregator.GetEvent<OpenItemMatchingSelectedBookIdEvent<Guid>>()
                        .Subscribe(OnOpenBookMatchingSelectedId);

                _eventAggregator.GetEvent<OpenItemMatchingSelectedPublisherIdEvent<Guid>>()
                        .Subscribe(OnOpenPublisherMatchingSelectedId);

                _eventAggregator.GetEvent<OpenItemMatchingSelectedAuthorIdEvent<Guid>>()
                    .Subscribe(OnOpenAuthorMatchingSelectedId);

                _eventAggregator.GetEvent<OpenItemMatchingSelectedSeriesIdEvent<Guid>>()
                    .Subscribe(OnOpenSeriesMatchingSelectedId);

                _eventAggregator.GetEvent<CloseDetailsViewEvent>()
                    .Subscribe(CloseDetailsView);

                _eventAggregator.GetEvent<ChangeDetailsViewEvent>()
                    .Subscribe(OnChangeDetailsView);
            }

            _eventAggregator.GetEvent<StatusMessageChangedEvent>().Subscribe(OnStatusMessageChanged);

            _eventAggregator.GetEvent<SearchEvent>().Subscribe(OnSearchSent);
        }

        private void OnSearchSent(SearchEventArgs obj)
        {
            var vm = _viewModelCreator["SearchViewModel"];
            (vm as SearchViewModel)!.SearchTerm = obj.SearchTerm;
            SelectedVm = vm;
            IsViewVisible = true;
        }

        public ICommand ShowMenuCommand { get; }
        public ICommand OpenSelectedViewCommand { get; }
        public ICommand CreateNewItemCommand { get; set; }

        public ObservableCollection<IDetailViewModel> DetailViewModels { get; }

        public IDetailViewModel SelectedDetailViewModel
        {
            get => _selectedDetailViewModel;
            set
            {
                _selectedDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        public bool IsViewVisible
        {
            get => _isViewVisible;
            set
            {
                _isViewVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsMenuBarVisible
        {
            get => _isMenuBarVisible;
            set
            {
                _isMenuBarVisible = SwitchMenuVisibility(value);
                OnPropertyChanged();
            }
        }

        private bool SwitchMenuVisibility(bool value)
        {
            PinGlyph = value ? '\uE718' : '\uE77A';

            return value;
        }

        public char PinGlyph
        {
            get => _pinGlyph;
            set
            {
                _pinGlyph = value;
                OnPropertyChanged();
            }
        }

        public ISelectedViewModel SelectedVm
        {
            get => _selectedVm;
            set
            {
                _selectedVm = value;
                OnPropertyChanged();
            }
        }

        private MessageItem _messageItem;

        public MessageItem MessageItem
        {
            get => _messageItem;
            set { _messageItem = value; OnPropertyChanged(); }
        }

        private bool _shouldAnimate;

        public bool ShouldAnimate
        {
            get => _shouldAnimate;
            set { _shouldAnimate = value; OnPropertyChanged(); }
        }

        private string _itemStatusCounter;

        public string ItemStatusCounter
        {
            get => _itemStatusCounter;
            set { _itemStatusCounter = value; OnPropertyChanged(); }
        }
        private void OnOpenSelectedItemView(OpenItemViewEventArgs args)
        {
            OnOpenSelectedViewExecute(args.ViewModelName);
        }

        private async void OnOpenDetailViewMatchingSelectedId(OpenDetailViewEventArgs args)
        {
            var detailViewModel = DetailViewModels
                .SingleOrDefault(vm => vm.Id == args.Id
                && vm.GetType().Name == args.ViewModelName);

            if (detailViewModel is null)
            {
                detailViewModel = _detailViewModelCreator[args.ViewModelName];
                try
                {
                    await detailViewModel.LoadAsync(args.Id);
                }
                catch (Exception ex)
                {
                    var dialog = new NotificationViewModel("Exception", ex.Message);
                    _dialogService.OpenDialog(dialog);

                    _logger.Error("Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.Message, ex.StackTrace);
                    return;
                }

                detailViewModel.IsQuickAdd = args.QuickAdd;
                DetailViewModels.Add(detailViewModel);
                SelectedDetailViewModel = DetailViewModels.Last();
            }
            else
            {
                SelectedDetailViewModel = DetailViewModels.SingleOrDefault(b => b.Id == args.Id);
            }

            IsViewVisible = false;
            ItemStatusCounter = "";
        }

        private void OnOpenSelectedViewExecute(string viewModel)
        {
            if (viewModel == nameof(MainPageViewModel))
            {
                ItemStatusCounter = "";
            }
            SelectedVm = _viewModelCreator[viewModel];
            IsViewVisible = true;
        }

        private void OnOpenBookMatchingSelectedId(Guid bookId)
        {
            OnOpenDetailViewMatchingSelectedId(
               new OpenDetailViewEventArgs
               {
                   Id = bookId,
                   ViewModelName = nameof(BookDetailViewModel)
               });
        }

        private void OnOpenPublisherMatchingSelectedId(Guid publisherId)
        {
            OnOpenDetailViewMatchingSelectedId(
                new OpenDetailViewEventArgs
                {
                    Id = publisherId,
                    ViewModelName = nameof(PublisherDetailViewModel)
                });
        }

        private void OnOpenAuthorMatchingSelectedId(Guid authorId)
        {
            OnOpenDetailViewMatchingSelectedId(
                new OpenDetailViewEventArgs
                {
                    Id = authorId,
                    ViewModelName = nameof(AuthorDetailViewModel)
                });
        }

        private void OnOpenSeriesMatchingSelectedId(Guid seriesId)
        {
            OnOpenDetailViewMatchingSelectedId(
                new OpenDetailViewEventArgs
                {
                    Id = seriesId,
                    ViewModelName = nameof(SeriesDetailViewModel)
                });
        }

        private void CloseDetailsView(CloseDetailsViewEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);

            if (DetailViewModels.Count == 0)
            {
                IsViewVisible = true;
            }
        }

        private void RemoveDetailViewModel(Guid id, string viewModelName)
        {
            var detailViewModel = DetailViewModels
                .SingleOrDefault(vm => vm.Id == id
                && vm.GetType().Name == viewModelName);

            if (detailViewModel != null)
            {
                DetailViewModels.Remove(detailViewModel);
            }

            SelectedDetailViewModel = null;
        }

        private void OnChangeDetailsView(ChangeDetailsViewEventArgs args)
        {
            ShouldAnimate = false;

            (SelectedVm as IItemLists)?.InitializeRepositoryAsync().Await();

            MessageItem = new MessageItem { Message = args.Message, MessageBackgroundColor = args.MessageBackgroundColor };

            ShouldAnimate = true;
        }

        private void OnCreateNewItemExecute(Type itemType)
        {
            if (itemType != null)
            {
                _eventAggregator.GetEvent<OpenDetailViewEvent>()
                               .Publish(new OpenDetailViewEventArgs
                               {
                                   Id = new Guid(),
                                   ViewModelName = Type.GetType(itemType.FullName).Name
                               });
            }
        }

        private void OnStatusMessageChanged(StatusMessageChangedEventArgs obj)
        {
            if (SelectedVm is IItemLists)
            {
                ItemStatusCounter = $"{obj.InfoText}: {obj.NumberOfItems} / {obj.AllItemsCount}";
            }
        }
    }
}
