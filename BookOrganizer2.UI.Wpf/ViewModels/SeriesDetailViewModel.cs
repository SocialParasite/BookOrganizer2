using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.Wpf.Enums;
using BookOrganizer2.UI.Wpf.Services;
using BookOrganizer2.UI.Wpf.Wrappers;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Serilog;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public sealed class SeriesDetailViewModel : BaseDetailViewModel<Series, SeriesId, SeriesWrapper>, IDropTarget
    {
        private SeriesWrapper _selectedItem;
        private readonly IBookLookupDataService _bookLookupDataService;

        public SeriesDetailViewModel(IEventAggregator eventAggregator,
                                     ILogger logger,
                                     ISeriesDomainService domainService,
                                     IBookLookupDataService bookLookupDataService,
                                     IDialogService dialogService)
            : base(eventAggregator, logger, domainService, dialogService)
        {
            _bookLookupDataService = bookLookupDataService ?? throw new ArgumentNullException(nameof(bookLookupDataService));

            AddSeriesPictureCommand = new DelegateCommand(OnAddSeriesPictureExecute);
            FilterBookListCommand = new DelegateCommand<string>(OnFilterBookListExecute);
            AddBookToSeriesCommand = new DelegateCommand<Guid?>(OnAddBookToSeriesExecute, OnAddBookToSeriesCanExecute);
            SaveItemCommand = new DelegateCommand(SaveItemExecute, SaveItemCanExecute)
                .ObservesProperty(() => SelectedItem.Name);
            SelectedItem = new SeriesWrapper(domainService.CreateItem());

            Books = new ObservableCollection<LookupItem>();
            AllBooks = new ObservableCollection<LookupItem>();
        }

        [UsedImplicitly]
        public ICommand AddSeriesPictureCommand { get; }
        [UsedImplicitly]
        public ICommand FilterBookListCommand { get; }
        [UsedImplicitly]
        public ICommand AddBookToSeriesCommand { get; }
        [UsedImplicitly]
        public ObservableCollection<LookupItem> Books { get; set; }
        [UsedImplicitly]
        public ObservableCollection<LookupItem> AllBooks { get; set; }

        public override SeriesWrapper SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value ?? throw new ArgumentNullException(nameof(SelectedItem));
                OnPropertyChanged();
            }
        }

        protected override string CreateChangeMessage(DatabaseOperation operation) 
            => $"{operation.ToString()}: {SelectedItem.Name}.";

        public override async Task LoadAsync(Guid id)
        {
            try
            {
                Series series = null;
                if (id != default)
                {
                    series = await ((ISeriesDomainService) DomainService).LoadAsync(id) ?? Series.NewSeries;
                }
                else
                {
                    series = Series.NewSeries;
                    IsNewItem = true;
                }
                SelectedItem = CreateWrapper(series);

                SelectedItem.PropertyChanged += (s, e) =>
                {
                    if (!HasChanges)
                    {
                        HasChanges = DomainService.HasChanges();
                    }
                    if (e.PropertyName == nameof(SelectedItem.HasErrors))
                    {
                        ((DelegateCommand)SaveItemCommand).RaiseCanExecuteChanged();
                    }
                    if (e.PropertyName == nameof(SelectedItem.Name))
                    {
                        TabTitle = SelectedItem.Name;
                    }
                };
                ((DelegateCommand)SaveItemCommand).RaiseCanExecuteChanged();

                Id = id;

                if (Id != default)
                {
                    TabTitle = SelectedItem.Name;
                }
                else
                {
                    SwitchEditableStateExecute();
                    SelectedItem.Name = "";
                }

                await PopulateAllBooksCollection();
                SetDefaultSeriesPictureIfNoneSet();

                void SetDefaultSeriesPictureIfNoneSet()
                {
                    SelectedItem.PicturePath ??= FileExplorerService.GetImagePath();
                }

                async Task PopulateAllBooksCollection()
                {
                    AllBooks.Clear();
                    foreach (var item in await GetBookList())
                    {
                        AllBooks.Add(item);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Error("Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.Message, ex.StackTrace);
            }
        }

        public override void SwitchEditableStateExecute()
        {
            base.SwitchEditableStateExecute();

            InitializeBookCollection();

            void InitializeBookCollection()
            {
                if (!Books.Any())
                {
                    var tempBookCollection = AllBooks.Where(item => !SelectedItem.Model.Books
                                                     .Any(x => x.SeriesId == SelectedItem.Id && x.BooksId == item.Id))
                                                     .OrderBy(b => b.DisplayMember);

                    PopulateBooksCollection(tempBookCollection);
                }
            }
        }

        public override void OnShowSelectedBookExecute(Guid? id)
        {
            if (UserMode.Item2 == DetailViewState.ViewMode)
            {
                base.OnShowSelectedBookExecute(id);
            }
            else
            {
                var booksSeries = SelectedItem.Model.Books
                    .First(s => s.SeriesId == SelectedItem.Id && s.BooksId == id);

                SelectedItem.Model.Books.Remove(booksSeries);

                Books.Add(new LookupItem { Id = booksSeries.BooksId, DisplayMember = booksSeries.Book.Title, Picture = booksSeries.Book.BookCoverPath });

                var countOfBooksInSeries = SelectedItem.Model.Books.Count();

                var inst = booksSeries.Instalment;
                if (inst < ++countOfBooksInSeries)
                {
                    foreach (var item in SelectedItem.Model.Books)
                    {
                        if (item.Instalment > inst)
                            item.Instalment--;
                    }
                }

                RefreshSeriesReadOrder();

                SetChangeTracker();
            }

        }

        private void RefreshSeriesReadOrder()
        {
            var temporaryReadOrderCollection = new ObservableCollection<ReadOrder>();
            temporaryReadOrderCollection.AddRange(SelectedItem.Model.Books.OrderBy(a => a.Instalment));
            SelectedItem.Model.Books.Clear();

            foreach (var item in temporaryReadOrderCollection)
            {
                SelectedItem.Model.Books.Add(item);
            }
        }

        private static bool OnAddBookToSeriesCanExecute(Guid? id)
            => id is not null && id != Guid.Empty;

        private void OnAddBookToSeriesExecute(Guid? id)
        {
            GetBookToAdd(id).Await();
        }

        private async Task GetBookToAdd(Guid? id)
        {
            var addedBook = await ((ISeriesDomainService)DomainService).GetBookAsync((Guid)id);

            SelectedItem.Model.Books
                .Add(new ReadOrder
                {
                    BooksId = addedBook.Id,
                    Book = addedBook,
                    Series = SelectedItem.Model,
                    SeriesId = SelectedItem.Id,
                    Instalment = SelectedItem.Model.Books.Count + 1
                });

            Books.Remove(Books.First(b => b.Id == id));

            SetChangeTracker();
        }

        private void OnFilterBookListExecute(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                var filteredCollection = AllBooks.Where(item => !SelectedItem.Model.Books
                                                 .Any(x => x.SeriesId == SelectedItem.Id && x.BooksId == item.Id))
                                                 .Where(item => item.DisplayMember
                                                    .IndexOf(filter, StringComparison.OrdinalIgnoreCase) != -1)
                                                 .OrderBy(b => b.DisplayMember);

                PopulateBooksCollection(filteredCollection);
            }
            else
            {
                var allExcludingBooksInSeries = AllBooks.Where(item => !SelectedItem.Model.Books
                                                        .Any(x => x.SeriesId == SelectedItem.Id && x.BooksId == item.Id))
                                                        .OrderBy(b => b.DisplayMember);

                PopulateBooksCollection(allExcludingBooksInSeries);
            }
        }

        private void PopulateBooksCollection(IOrderedEnumerable<LookupItem> tempBookCollection)
        {
            if (Books.Count == 0 && !tempBookCollection.Any())
            {
                Books = AllBooks;
            }
            else
            {
                Books.Clear();
                foreach (var item in tempBookCollection)
                {
                    Books.Add(item);
                }
            }
        }

        private Task<IEnumerable<BookLookupItem>> GetBookList()
            => _bookLookupDataService.GetBookLookupAsync(nameof(BookDetailViewModel));

        private void OnAddSeriesPictureExecute()
        {
            var temp = SelectedItem.PicturePath;
            SelectedItem.PicturePath = FileExplorerService.BrowsePicture() ?? SelectedItem.PicturePath;
            if (!string.IsNullOrEmpty(SelectedItem.PicturePath)
                && SelectedItem.PicturePath != temp)
            {
                FileExplorerService.CreateThumbnail(SelectedItem.PicturePath, DialogService);
            }
        }

        public override SeriesWrapper CreateWrapper(Series entity)
        {
            var wrapper = new SeriesWrapper(entity);
            return wrapper;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is ReadOrder _ && dropInfo.TargetItem is ReadOrder _)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            ReadOrder sourceItem = dropInfo.Data as ReadOrder;
            ReadOrder targetItem = dropInfo.TargetItem as ReadOrder;

            var originalSourceInstalment = sourceItem.Instalment;
            var originalTargetInstalment = targetItem.Instalment;


            if (originalTargetInstalment == (originalSourceInstalment + 1)
                || originalTargetInstalment == (originalSourceInstalment - 1))
            {
                originalSourceInstalment = sourceItem.Instalment;

                targetItem.Instalment = originalSourceInstalment;
                sourceItem.Instalment = originalTargetInstalment;

                SelectedItem.Model.Books.Remove(sourceItem);
                SelectedItem.Model.Books.Add(sourceItem);
                SelectedItem.Model.Books.Remove(targetItem);
                SelectedItem.Model.Books.Add(targetItem);
            }
            else
            {
                sourceItem.Instalment = targetItem.Instalment;
                SelectedItem.Model.Books.Remove(sourceItem);

                foreach (var item in SelectedItem.Model.Books)
                {
                    if (item.Instalment > originalSourceInstalment && item.Instalment <= sourceItem.Instalment)
                    {
                        item.Instalment--;
                    }
                    else if (item.Instalment < originalSourceInstalment && item.Instalment >= sourceItem.Instalment)
                    {
                        item.Instalment++;
                    }
                }

                SelectedItem.Model.Books.Add(sourceItem);

                RefreshSeriesReadOrder();
            }
            SetChangeTracker();
        }
    }
}
