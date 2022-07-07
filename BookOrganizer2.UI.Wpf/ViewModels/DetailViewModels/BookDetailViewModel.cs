using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.Common;
using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using BookOrganizer2.UI.Wpf.Enums;
using BookOrganizer2.UI.Wpf.Events;
using BookOrganizer2.UI.Wpf.Services;
using BookOrganizer2.UI.Wpf.Wrappers;
using Prism.Commands;
using Prism.Events;
using Serilog;

namespace BookOrganizer2.UI.Wpf.ViewModels.DetailViewModels
{
    public class BookDetailViewModel : BaseDetailViewModel<Book, BookId, BookWrapper>
    {
        // TODO: ???
        private bool _languageIsDirty; // _navPropertyIsDirty; ?? Is one enough for all?

        private bool LanguageIsDirty
        {
            get => _languageIsDirty;
            set { _languageIsDirty = value; OnPropertyChanged(); }
        }

        private bool _publisherIsDirty; // _navPropertyIsDirty; ?? Is one enough for all?

        private bool PublisherIsDirty
        {
            get => _publisherIsDirty;
            set { _publisherIsDirty = value; OnPropertyChanged(); }
        }

        private BookWrapper _selectedItem;
        private SolidColorBrush _highlightBrush;
        private DateTime _newReadDate;
        private LookupItem _selectedLanguage;
        private LookupItem _selectedPublisher;
        private LookupItem _selectedAuthor;
        private Guid _selectedPublisherId;
        private Guid _selectedAuthorId;
        private Guid _selectedSeriesId;
        private int _selectedReleaseYear;
        private string _newFormatName;
        private string _newGenreName;

        public BookDetailViewModel(IEventAggregator eventAggregator,
            ILogger logger,
            IBookDomainService domainService,
            IDialogService dialogService)
            : base(eventAggregator, logger, domainService, dialogService)
        {
            HighlightMouseOverCommand = new DelegateCommand(HighlightMouseOverExecute);
            HighlightMouseLeaveCommand = new DelegateCommand(HighlightMouseLeaveExecute);
            SetReadDateCommand = new DelegateCommand(SetReadDateExecute, SetReadDateCanExecute)
                .ObservesProperty(() => NewReadDate);
            AddBookCoverImageCommand = new DelegateCommand(AddBookCoverImageExecute);
            AddAuthorAsABookAuthorCommand = new DelegateCommand<LookupItem>(AddBookAuthorExecute);
            AddNewAuthorCommand = new DelegateCommand(OnAddNewAuthorExecute);
            AddNewPublisherCommand = new DelegateCommand(OnAddNewPublisherExecute);
            AddNewLanguageCommand = new DelegateCommand(OnAddNewLanguageExecute);
            RemoveAuthorAsABookAuthorCommand = new DelegateCommand<Guid?>(RemoveAuthorExecute);
            LanguageSelectionChangedCommand = new DelegateCommand(OnLanguageSelectionChangedExecute);
            PublisherSelectionChangedCommand = new DelegateCommand(OnPublisherSelectionChangedExecute);
            RemoveDateAsABookReadDateCommand = new DelegateCommand<DateTime?>(OnRemoveDateAsABookReadDateExecute);
            ReleaseYearSelectionChangedCommand = new DelegateCommand(OnReleaseYearSelectionChangedExecute);
            ShowSelectedPublisherCommand
                = new DelegateCommand<Guid?>(OnShowSelectedPublisherExecute, OnShowSelectedPublisherCanExecute);
            ShowSelectedAuthorCommand = new DelegateCommand<Guid?>(OnShowSelectedAuthorExecute, OnShowSelectedAuthorCanExecute);
            ShowSelectedSeriesCommand = new DelegateCommand<Guid?>(OnShowSelectedSeriesExecute, OnShowSelectedSeriesCanExecute);
            BookFormatSelectionChangedCommand = new DelegateCommand<LookupItem>(OnBookFormatSelectionChangedExecute);
            BookGenreSelectionChangedCommand = new DelegateCommand<LookupItem>(OnBookGenreSelectionChangedExecute);
            AddNewFormatCommand = new DelegateCommand<string>(OnAddNewFormatExecute, OnAddNewFormatCanExecute)
                .ObservesProperty(() => NewFormatName);
            AddNewGenreCommand = new DelegateCommand<string>(OnAddNewGenreExecute, OnAddNewGenreCanExecute)
                .ObservesProperty(() => NewGenreName);
            SaveItemCommand = new DelegateCommand(SaveItemExecute, SaveItemCanExecute)
                .ObservesProperty(() => SelectedItem.Title)
                .ObservesProperty(() => SelectedItem.PageCount)
                .ObservesProperty(() => LanguageIsDirty)
                .ObservesProperty(() => PublisherIsDirty);
            AddNewNoteCommand = new DelegateCommand(OnAddNewNoteExecute);
            RemoveNoteCommand = new DelegateCommand<Note>(OnRemoveNoteExecute);

            SelectedItem = new BookWrapper(domainService.CreateItem());

            NewReadDate = DateTime.Today;
            Languages = new ObservableCollection<LookupItem>();
            Publishers = new ObservableCollection<LookupItem>();
            Authors = new ObservableCollection<LookupItem>();
            AllBookFormats = new ObservableCollection<Tuple<LookupItem, bool>>();
            AllBookGenres = new ObservableCollection<Tuple<LookupItem, bool>>();

            YearsList = PopulateYearsMenu();
        }

        private async void OnNewLanguageAdded(NewLanguageEventArgs obj) 
            => await InitializeLanguageCollection(true);

        private async void OnNewPublisherAdded(NewPublisherEventArgs obj) 
            => await InitializePublisherCollection(true);

        private async void OnNewAuthorAdded(NewAuthorEventArgs obj) 
            => await InitializeAuthorCollection(true);

        private void OnRemoveNoteExecute(Note note)
        {
            SelectedItem.Model.Notes.Remove(note);
            SetChangeTracker();
        }

        private void OnAddNewNoteExecute()
        {
            SelectedItem.Model.Notes.Add(Note.NewNote);
            SetChangeTracker();
        }

        public ICommand HighlightMouseLeaveCommand { get; }
        public ICommand HighlightMouseOverCommand { get; }
        public ICommand SetReadDateCommand { get; }
        public ICommand AddBookCoverImageCommand { get; }
        public ICommand AddNewAuthorCommand { get; }
        public ICommand AddNewPublisherCommand { get; }
        public ICommand AddNewLanguageCommand { get; }
        public ICommand AddAuthorAsABookAuthorCommand { get; }
        public ICommand RemoveAuthorAsABookAuthorCommand { get; }
        public ICommand LanguageSelectionChangedCommand { get; }
        public ICommand PublisherSelectionChangedCommand { get; }
        public ICommand RemoveDateAsABookReadDateCommand { get; }
        public ICommand ReleaseYearSelectionChangedCommand { get; }
        public ICommand ShowSelectedPublisherCommand { get; }
        public ICommand ShowSelectedAuthorCommand { get; }
        public ICommand ShowSelectedSeriesCommand { get; }
        public ICommand BookFormatSelectionChangedCommand { get; }
        public ICommand BookGenreSelectionChangedCommand { get; }
        public ICommand AddNewFormatCommand { get; }
        public ICommand AddNewGenreCommand { get; }
        public ICommand AddNewNoteCommand { get; }
        public ICommand RemoveNoteCommand { get; }

        public Guid SelectedPublisherId
        {
            get => _selectedPublisherId;
            set
            {
                _selectedPublisherId = value;
                OnPropertyChanged();

                if (_selectedPublisherId != Guid.Empty)
                {
                    EventAggregator.GetEvent<OpenItemMatchingSelectedPublisherIdEvent<Guid>>()
                                   .Publish(SelectedPublisherId);
                }
            }
        }

        public Guid SelectedAuthorId
        {
            get => _selectedAuthorId;
            set
            {
                _selectedAuthorId = value;
                OnPropertyChanged();

                if (_selectedAuthorId != Guid.Empty)
                {
                    EventAggregator.GetEvent<OpenItemMatchingSelectedAuthorIdEvent<Guid>>()
                                   .Publish(SelectedAuthorId);
                }
            }
        }

        public Guid SelectedSeriesId
        {
            get => _selectedSeriesId;
            set
            {
                _selectedSeriesId = value;
                OnPropertyChanged();

                if (_selectedSeriesId != Guid.Empty)
                {
                    EventAggregator.GetEvent<OpenItemMatchingSelectedSeriesIdEvent<Guid>>()
                                   .Publish(SelectedSeriesId);
                }
            }
        }

        public SolidColorBrush HighlightBrush
        {
            get => _highlightBrush;
            set { _highlightBrush = value; OnPropertyChanged(); }
        }

        public ObservableCollection<LookupItem> Languages { get; set; }

        public LookupItem SelectedLanguage
        {
            get => _selectedLanguage;
            set { _selectedLanguage = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Tuple<LookupItem, bool>> AllBookFormats { get; set; }

        public ObservableCollection<Tuple<LookupItem, bool>> AllBookGenres { get; set; }

        public ObservableCollection<LookupItem> Publishers { get; set; }

        public LookupItem SelectedPublisher
        {
            get => _selectedPublisher;
            set { _selectedPublisher = value; OnPropertyChanged(); }
        }

        public ObservableCollection<LookupItem> Authors { get; set; }

        public LookupItem SelectedAuthor
        {
            get => _selectedAuthor;
            set { _selectedAuthor = value; OnPropertyChanged(); }
        }

        public DateTime NewReadDate
        {
            get => _newReadDate;
            set { _newReadDate = value; OnPropertyChanged(); }
        }

        public IEnumerable<int> YearsList { get; set; }

        public int SelectedReleaseYear
        {
            get => _selectedReleaseYear;
            set { _selectedReleaseYear = value; OnPropertyChanged(); }
        }

        public override BookWrapper SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value ?? throw new ArgumentNullException(nameof(SelectedItem));
                OnPropertyChanged();
            }
        }

        public string NewFormatName
        {
            get => _newFormatName;
            set { _newFormatName = value; OnPropertyChanged(); }
        }

        public string NewGenreName
        {
            get => _newGenreName;
            set { _newGenreName = value; OnPropertyChanged(); }
        }

        private void HighlightMouseLeaveExecute()
           => HighlightBrush = Brushes.White;

        private void HighlightMouseOverExecute()
            => HighlightBrush = Brushes.LightSkyBlue;


        private bool SetReadDateCanExecute()
        {
            return SelectedItem.Model.ReadDates.All(d => d.ReadDate != NewReadDate);
        }

        private void SetReadDateExecute()
        {
            var newReadDate = new BookReadDate(NewReadDate);

            SelectedItem.Model.ReadDates.Add(newReadDate);
            SetChangeTracker();

        }

        private void AddBookCoverImageExecute()
        {
            var temp = SelectedItem.BookCoverPath;
            SelectedItem.BookCoverPath = FileExplorerService.BrowsePicture() ?? SelectedItem.BookCoverPath;
            if (!string.IsNullOrEmpty(SelectedItem.BookCoverPath)
                && SelectedItem.BookCoverPath != temp)
            {
                FileExplorerService.CreateThumbnail(SelectedItem.BookCoverPath, DialogService);
            }
        }

        public override async Task LoadAsync(Guid id)
        {
            try
            {
                Book book;

                if (id != default)
                {
                    book = await ((IBookDomainService)DomainService).LoadAsync(id).ConfigureAwait(false);
                }
                else
                {
                    book = Book.NewBook;
                    IsNewItem = true;
                }

                SelectedItem = CreateWrapper(book);

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
                    if (e.PropertyName == nameof(SelectedItem.Title))
                    {
                        SetTabTitle();
                    }
                };
                ((DelegateCommand)SaveItemCommand).RaiseCanExecuteChanged();

                Id = id;

                if (Id != default)
                {
                    SetTabTitle();
                }
                else
                {
                    SwitchEditableStateExecute();
                    SelectedItem.Title = "";
                    Id = book.Id;
                }

                SetDefaultBookCoverIfNoneSet();
                SetDefaultBookTitleIfNoneSet();
                InitializeSelectedLanguageIfNoneSet();
                InitializeSelectedPublisherIfNoneSet();
                SetBooksSelectedReleaseYear();
                InitializeAllBookFormats();
                InitializeAllBookGenres();

                void SetDefaultBookCoverIfNoneSet()
                {
                    SelectedItem.BookCoverPath ??= FileExplorerService.GetImagePath();
                }

                void SetDefaultBookTitleIfNoneSet()
                {
                    if (string.IsNullOrWhiteSpace(SelectedItem.Title))
                        SelectedItem.Title = "Book Title";
                }

                void InitializeSelectedLanguageIfNoneSet()
                {
                    if (SelectedLanguage is not null) return;
                    if (SelectedItem.Model.Language is not null)
                    {
                        SelectedLanguage =
                            new LookupItem
                            {
                                Id = SelectedItem.Model.Language.Id,
                                DisplayMember = SelectedItem.Model.Language == null
                                    ? Language.NewLanguage.Name
                                    : SelectedItem.Model.Language.Name
                            };
                    }
                }

                void InitializeSelectedPublisherIfNoneSet()
                {
                    if (SelectedPublisher is not null) return;
                    if (SelectedItem.Model.Publisher is not null)
                    {
                        SelectedPublisher =
                            new LookupItem
                            {
                                Id = SelectedItem.Model.Publisher.Id,
                                DisplayMember = SelectedItem.Model.Publisher == null
                                    ? Publisher.NewPublisher.Name
                                    : SelectedItem.Model.Publisher.Name
                            };
                    }
                }

                void SetBooksSelectedReleaseYear()
                {
                    SelectedReleaseYear = SelectedItem.ReleaseYear == 0
                        ? DateTime.Today.Year
                        : SelectedItem.ReleaseYear;
                }

                void InitializeAllBookFormats()
                {
                    if (SelectedItem.Model.Formats == null || SelectedItem.Model.Formats.Count == 0) return;
                    if (AllBookFormats.Any()) return;

                    AllBookFormats.Clear();

                    foreach (var item in SelectedItem.Model.Formats)
                    {
                        var newLookupItem = new LookupItem { Id = item.Id, DisplayMember = item.Name };
                        AllBookFormats.Add((newLookupItem, true).ToTuple());
                    }
                }

                void InitializeAllBookGenres()
                {
                    if (SelectedItem.Model.Genres == null || SelectedItem.Model.Genres.Count == 0) return;
                    if (AllBookGenres.Any()) return;

                    AllBookGenres.Clear();

                    foreach (var item in SelectedItem.Model.Genres)
                    {
                        var newLookupItem = new LookupItem { Id = item.Id, DisplayMember = item.Name };
                        AllBookGenres.Add((newLookupItem, true).ToTuple());
                    }
                }

                void SetTabTitle()
                    => TabTitle = $"{SelectedItem.Model.Title}";

            }
            catch (Exception ex)
            {
                var dialog = new NotificationViewModel("Exception", ex.Message);
                DialogService.OpenDialog(dialog);

                Logger.Error(ex, "Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.Message, ex.StackTrace);
            }
        }

        protected override bool SaveItemCanExecute()
            => (!SelectedItem.HasErrors) && (HasChanges || IsNewItem || LanguageIsDirty || PublisherIsDirty);

        protected override void SaveItemExecute() => SaveItem().Await();

        private async Task SaveItem()
        {
            if (PublisherIsDirty)
            {
                var currentPublisher =
                    await ((IBookDomainService)DomainService).GetPublisherAsync(SelectedPublisher.Id);
                SelectedItem.Model.SetPublisher(currentPublisher);
            }
            if (LanguageIsDirty)
            {
                var currentLanguage =
                    await ((IBookDomainService)DomainService).GetLanguageAsync(SelectedLanguage.Id);
                SelectedItem.Model.SetLanguage(currentLanguage);
            }
            base.SaveItemExecute();
        }

        protected override string CreateChangeMessage(DatabaseOperation operation)
            => $"{operation.ToString()}: {SelectedItem.Title}.";

        public override void SwitchEditableStateExecute() => SwitchStateAsync().Await();

        private async Task SwitchStateAsync()
        {
            base.SwitchEditableStateExecute();

            if (!UserMode.Item1)
            {
                SubscribeToEvents();
            }
            else
            {
                UnsubscribeEvents();
            }

            await InitializeLanguageCollection();
            await InitializePublisherCollection();
            await InitializeAuthorCollection();
            await InitializeAllBookFormatsCollection();
            await InitializeAllBookGenresCollection();

            SelectedItem.ReleaseYear = SelectedReleaseYear == 0 ? 1 : SelectedReleaseYear;

            void UnsubscribeEvents()
            {
                EventAggregator.GetEvent<NewAuthorEvent>().Unsubscribe(OnNewAuthorAdded);
                EventAggregator.GetEvent<NewPublisherEvent>().Unsubscribe(OnNewPublisherAdded);
                EventAggregator.GetEvent<NewLanguageEvent>().Unsubscribe(OnNewLanguageAdded);
            }

            void SubscribeToEvents()
            {
                EventAggregator.GetEvent<NewAuthorEvent>()
                    .Subscribe(OnNewAuthorAdded);
                EventAggregator.GetEvent<NewPublisherEvent>()
                    .Subscribe(OnNewPublisherAdded);
                EventAggregator.GetEvent<NewLanguageEvent>()
                    .Subscribe(OnNewLanguageAdded);
            }
        }

        async Task InitializeLanguageCollection(bool reset = false)
        {
            if (!Languages.Any() || reset)
            {
                if (reset)
                {
                    while (await ((BookService)DomainService).GetLanguageCount() == Languages.Count)
                    {
                    }
                }

                Languages.Clear();

                foreach (var item in await GetLanguageList())
                {
                    Languages.Add(item);
                }

                if (SelectedItem.Model.Language != null)
                    SelectedLanguage = Languages.SingleOrDefault(l => l.Id == SelectedItem.Model.Language.Id);
            }
        }

        async Task InitializePublisherCollection(bool reset = false)
        {
            if (!Publishers.Any() || reset)
            {
                if (reset)
                {
                    while (await ((BookService)DomainService).GetPublisherCount() == Publishers.Count)
                    {
                    }
                }
                Publishers.Clear();

                foreach (var item in await GetPublisherList())
                {
                    Publishers.Add(item);
                }

                if (SelectedItem.Model.Publisher != null)
                    SelectedPublisher = Publishers.SingleOrDefault(p => p.Id == SelectedItem.Model.Publisher.Id);
            }
        }

        async Task InitializeAuthorCollection(bool reset = false)
        {
            if (!Authors.Any() || reset)
            {
                if (reset)
                {
                    while (await ((BookService)DomainService).GetAuthorCount() == Authors.Count)
                    {
                    }
                }
                Authors.Clear();

                foreach (var item in await GetAuthorList())
                {
                    if (SelectedItem.Model.Authors.All(a => a.Id != item.Id))
                        Authors.Add(item);
                }
            }
        }

        async Task InitializeAllBookFormatsCollection()
        {
            if (!UserMode.Item1)
            {
                foreach (var item in await GetBookFormatList())
                {
                    if (AllBookFormats.Any(i => i.Item1.Id == item.Id))
                    {
                        continue;
                    }
                    AllBookFormats.Add((item, false).ToTuple());
                }
            }
            else
            {
                var tempBookFormatsCollection = new ObservableCollection<Tuple<LookupItem, bool>>();
                foreach (var item in AllBookFormats)
                {
                    if (item.Item2)
                    {
                        tempBookFormatsCollection.Add(item);
                    }
                }
                AllBookFormats.Clear();

                foreach (var item in tempBookFormatsCollection)
                {
                    AllBookFormats.Add(item);
                }
            }
        }

        async Task InitializeAllBookGenresCollection()
        {
            if (!UserMode.Item1)
            {
                foreach (var item in await GetBookGenreList())
                {
                    if (AllBookGenres.Any(i => i.Item1.Id == item.Id))
                    {
                        continue;
                    }
                    AllBookGenres.Add((item, false).ToTuple());
                }
            }
            else
            {
                var tempBookGenresCollection = new ObservableCollection<Tuple<LookupItem, bool>>();
                foreach (var item in AllBookGenres)
                {
                    if (item.Item2)
                    {
                        tempBookGenresCollection.Add(item);
                    }
                }
                AllBookGenres.Clear();

                foreach (var item in tempBookGenresCollection)
                {
                    AllBookGenres.Add(item);
                }
            }
        }

        private Task<IEnumerable<LookupItem>> GetPublisherList()
            => (DomainService as BookService)!.GetPublisherLookupAsync(nameof(PublisherDetailViewModel))
;

        private Task<IEnumerable<LookupItem>> GetLanguageList()
            => (DomainService as BookService)!.GetLanguageLookupAsync(nameof(LanguageDetailViewModel))
;

        private Task<IEnumerable<LookupItem>> GetAuthorList()
            => (DomainService as BookService)!.GetAuthorLookupAsync(nameof(AuthorDetailViewModel))
;

        private Task<IEnumerable<LookupItem>> GetBookFormatList()
            => (DomainService as BookService)!.GetFormatLookupAsync(nameof(FormatDetailViewModel));

        private Task<IEnumerable<LookupItem>> GetBookGenreList()
            => (DomainService as BookService)!.GetGenreLookupAsync(nameof(GenreDetailViewModel));

        private void RemoveAuthorExecute(Guid? authorId)
        {
            RemoveAuthorAsync(authorId).Await();
        }

        private async Task RemoveAuthorAsync(Guid? authorId)
        {
            if (authorId is not null)
            {
                var removedAuthor = await ((IBookDomainService)DomainService).GetAuthorAsync((Guid)authorId);
                Authors.Add(
                    new LookupItem
                    {
                        Id = (Guid)authorId,
                        DisplayMember = $"{removedAuthor.LastName}, {removedAuthor.FirstName}"
                    });

                var temporaryAuthorCollection = new ObservableCollection<LookupItem>();
                temporaryAuthorCollection.AddRange(Authors.OrderBy(a => a.DisplayMember));
                Authors.Clear();

                foreach (var item in temporaryAuthorCollection)
                {
                    Authors.Add(item);
                }

                var removedAuthorAsLookupItem = SelectedItem.Model.Authors.Single(al => al.Id == authorId);
                SelectedItem.Model.Authors.Remove(removedAuthorAsLookupItem);

                SetChangeTracker();
            }
        }

        private void AddBookAuthorExecute(LookupItem lookupItem)
        {
            if (lookupItem is null) return;

            AddAuthorAsync(lookupItem).Await();
        }

        private async Task AddAuthorAsync(LookupItem lookupItem)
        {
            var addedAuthor = await ((IBookDomainService)DomainService).GetAuthorAsync(lookupItem.Id);

            SelectedItem.Model.Authors.Add(addedAuthor);

            Authors.Remove(lookupItem);

            SetChangeTracker();
        }

        private void OnLanguageSelectionChangedExecute()
        {
            if (SelectedLanguage is not null && Languages.Any())
            {
                LanguageIsDirty = SelectedItem.Model.Language is null ||
                                     SelectedItem.Model.Language?.Id != SelectedLanguage.Id;
            }
        }

        private void OnPublisherSelectionChangedExecute()
        {
            if (SelectedPublisher is not null && Publishers.Any())
            {
                PublisherIsDirty = SelectedItem.Model.Publisher is null ||
                                  SelectedItem.Model.Publisher?.Id != SelectedPublisher.Id;
            }
        }

        private void OnBookFormatSelectionChangedExecute(LookupItem lookupItem)
        {
            EditBooksFormatsAsync(lookupItem).Await();
        }

        private async Task EditBooksFormatsAsync(LookupItem lookupItem)
        {
            if (AllBookFormats.Any(bf => bf.Item1.Id == lookupItem.Id && !bf.Item2))
            {
                AllBookFormats.Remove(AllBookFormats.Single(bf => bf.Item1.Id == lookupItem.Id));
                AllBookFormats.Add((lookupItem, true).ToTuple());

                var newFormat = await ((IBookDomainService)DomainService).GetFormatAsync(lookupItem.Id);

                SelectedItem.Model.Formats.Add(newFormat);
            }
            else if (AllBookFormats.Any(bf => bf.Item1.Id == lookupItem.Id && bf.Item2))
            {
                AllBookFormats.Remove(AllBookFormats.Single(bf => bf.Item1.Id == lookupItem.Id));
                AllBookFormats.Add((lookupItem, false).ToTuple());

                var item = SelectedItem.Model.Formats.Single(f => f.Id == lookupItem.Id);
                SelectedItem.Model.Formats.Remove(item);
            }

            SetChangeTracker();
        }

        private void OnBookGenreSelectionChangedExecute(LookupItem lookupItem)
        {
            EditBooksGenresAsync(lookupItem).Await();
        }

        private async Task EditBooksGenresAsync(LookupItem lookupItem)
        {
            if (AllBookGenres.Any(bg => bg.Item1.Id == lookupItem.Id && !bg.Item2))
            {
                AllBookGenres.Remove(AllBookGenres.Single(bg => bg.Item1.Id == lookupItem.Id));
                AllBookGenres.Add((lookupItem, true).ToTuple());

                var newGenre = await ((IBookDomainService)DomainService).GetGenreAsync(lookupItem.Id);

                SelectedItem.Model.Genres.Add(newGenre);
            }
            else if (AllBookGenres.Any(bg => bg.Item1.Id == lookupItem.Id && bg.Item2))
            {
                AllBookGenres.Remove(AllBookGenres.Single(bg => bg.Item1.Id == lookupItem.Id));
                AllBookGenres.Add((lookupItem, false).ToTuple());

                var item = SelectedItem.Model.Genres.Single(g => g.Id == lookupItem.Id);
                SelectedItem.Model.Genres.Remove(item);
            }

            SetChangeTracker();
        }

        private bool OnShowSelectedAuthorCanExecute(Guid? id)
            => id is not null && id != Guid.Empty;

        private void OnShowSelectedAuthorExecute(Guid? id)
            => SelectedAuthorId = (Guid)id;

        private bool OnShowSelectedPublisherCanExecute(Guid? id)
            => id is not null && id != Guid.Empty;

        private void OnShowSelectedPublisherExecute(Guid? id)
            => SelectedPublisherId = (Guid)id;

        private void OnShowSelectedSeriesExecute(Guid? id)
            => SelectedSeriesId = (Guid)id;

        private bool OnShowSelectedSeriesCanExecute(Guid? id)
            => id is not null && id != Guid.Empty;

        private void OnRemoveDateAsABookReadDateExecute(DateTime? readDate)
        {
            if (SelectedItem.Model.ReadDates.Any(d => d.ReadDate == readDate))
            {
                var deletedReadDate = SelectedItem.Model.ReadDates.First(rd => rd.ReadDate == readDate);
                SelectedItem.Model.ReadDates.Remove(deletedReadDate);

                SetChangeTracker();
            }
        }

        private void OnReleaseYearSelectionChangedExecute()
            => SelectedItem.ReleaseYear = SelectedReleaseYear;

        private IEnumerable<int> PopulateYearsMenu()
        {
            for (var year = DateTime.Today.Year; year > 0; year--)
                yield return year;
        }

        private void OnAddNewAuthorExecute()
        {
            EventAggregator.GetEvent<OpenDetailViewEvent>()
                           .Publish(new OpenDetailViewEventArgs
                           {
                               Id = new Guid(),
                               ViewModelName = nameof(AuthorDetailViewModel),
                               QuickAdd = true
                           });

        }
        private void OnAddNewPublisherExecute()
        {
            EventAggregator.GetEvent<OpenDetailViewEvent>()
                           .Publish(new OpenDetailViewEventArgs
                           {
                               Id = new Guid(),
                               ViewModelName = nameof(PublisherDetailViewModel)
                           });
        }
        private void OnAddNewLanguageExecute()
        {
            EventAggregator.GetEvent<OpenDetailViewEvent>()
                           .Publish(new OpenDetailViewEventArgs
                           {
                               Id = new Guid(),
                               ViewModelName = nameof(LanguageDetailViewModel)
                           });
        }

        public override BookWrapper CreateWrapper(Book entity) => new BookWrapper(entity);

        private bool OnAddNewFormatCanExecute(string formatName)
        {
            if (string.IsNullOrEmpty(formatName))
            {
                return false;
            }
            return !AllBookFormats.Any(f
                => f.Item1.DisplayMember.Equals(formatName, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool OnAddNewGenreCanExecute(string genreName)
        {
            if (string.IsNullOrEmpty(genreName))
            {
                return false;
            }
            return !AllBookGenres.Any(f
                => f.Item1.DisplayMember.Equals(genreName, StringComparison.InvariantCultureIgnoreCase));
        }

        private void OnAddNewGenreExecute(string genre) => AddNewGenre(genre).Await();

        private async Task AddNewGenre(string genre)
        {
            await ((IBookDomainService)DomainService).AddNewGenre(genre);
            await InitializeAllBookGenresCollection();
        }

        private void OnAddNewFormatExecute(string format) => AddNewFormat(format).Await();

        private async Task AddNewFormat(string format)
        {
            await ((IBookDomainService)DomainService).AddNewFormat(format);
            await InitializeAllBookFormatsCollection();
        }
    }
}
