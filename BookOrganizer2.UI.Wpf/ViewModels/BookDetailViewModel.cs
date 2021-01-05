﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using BookOrganizer2.DA.Repositories;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.DA;
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

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class BookDetailViewModel : BaseDetailViewModel<Book, BookId, BookWrapper>
    {
        private BookWrapper _selectedItem;

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

        private BookWrapper selectedItem;
        private SolidColorBrush highlightBrush;
        private DateTime newReadDate;
        private LookupItem selectedLanguage;
        private LookupItem selectedPublisher;
        private LookupItem selectedAuthor;
        private Guid selectedPublisherId;
        private Guid selectedAuthorId;
        private Guid selectedSeriesId;
        private int selectedReleaseYear;
        private string newFormatName;
        private string newGenreName;

        public BookDetailViewModel(IEventAggregator eventAggregator,
            ILogger logger,
            IDomainService<Book, BookId> domainService,
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
            //ShowSelectedSeriesCommand = new DelegateCommand<Guid?>(OnShowSelectedSeriesExecute, OnShowSelectedSeriesCanExecute);
            BookFormatSelectionChangedCommand = new DelegateCommand<LookupItem>(OnBookFormatSelectionChangedExecute);
            BookGenreSelectionChangedCommand = new DelegateCommand<LookupItem>(OnBookGenreSelectionChangedExecute);
            AddNewFormatCommand = new DelegateCommand<string>(OnAddNewFormatExecute, OnAddNewFormatCanExecute)
                .ObservesProperty(() => NewFormatName);
            AddNewGenreCommand = new DelegateCommand<string>(OnAddNewGenreExecute, OnAddNewGenreCanExecute)
                .ObservesProperty(() => NewGenreName);
            SaveItemCommand = new DelegateCommand(SaveItemExecute, SaveItemCanExecute)
                .ObservesProperty(() => SelectedItem.Title)
                .ObservesProperty(() => SelectedItem.PageCount);
                //.ObservesProperty(() => SelectedItem.LanguageId)
                //.ObservesProperty(() => SelectedItem.PublisherId);

            SelectedItem = new BookWrapper(domainService.CreateItem());

            NewReadDate = DateTime.Today;
            Languages = new ObservableCollection<LookupItem>();
            Publishers = new ObservableCollection<LookupItem>();
            Authors = new ObservableCollection<LookupItem>();
            AllBookFormats = new ObservableCollection<Tuple<LookupItem, bool>>();
            AllBookGenres = new ObservableCollection<Tuple<LookupItem, bool>>();

            YearsList = PopulateYearsMenu();
        }

        public ICommand HighlightMouseLeaveCommand { get; }
        public ICommand HighlightMouseOverCommand { get; }
        public ICommand SetReadDateCommand { get; set; }
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

        public Guid SelectedPublisherId
        {
            get => selectedPublisherId;
            set
            {
                selectedPublisherId = value;
                OnPropertyChanged();

                if (selectedPublisherId != Guid.Empty)
                {
                    EventAggregator.GetEvent<OpenItemMatchingSelectedPublisherIdEvent<Guid>>()
                                   .Publish(SelectedPublisherId);
                }
            }
        }

        public Guid SelectedAuthorId
        {
            get => selectedAuthorId;
            set
            {
                selectedAuthorId = value;
                OnPropertyChanged();

                if (selectedAuthorId != Guid.Empty)
                {
                    EventAggregator.GetEvent<OpenItemMatchingSelectedAuthorIdEvent<Guid>>()
                                   .Publish(SelectedAuthorId);
                }
            }
        }

        //public Guid SelectedSeriesId
        //{
        //    get => selectedSeriesId;
        //    set
        //    {
        //        selectedSeriesId = value;
        //        OnPropertyChanged();

        //        if (selectedSeriesId != Guid.Empty)
        //        {
        //            EventAggregator.GetEvent<OpenItemMatchingSelectedSeriesIdEvent<Guid>>()
        //                           .Publish(SelectedSeriesId);
        //        }
        //    }
        //}

        public SolidColorBrush HighlightBrush
        {
            get => highlightBrush;
            set { highlightBrush = value; OnPropertyChanged(); }
        }

        public ObservableCollection<LookupItem> Languages { get; set; }

        public LookupItem SelectedLanguage
        {
            get => selectedLanguage;
            set { selectedLanguage = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Tuple<LookupItem, bool>> AllBookFormats { get; set; }

        public ObservableCollection<Tuple<LookupItem, bool>> AllBookGenres { get; set; }

        public ObservableCollection<LookupItem> Publishers { get; set; }

        public LookupItem SelectedPublisher
        {
            get => selectedPublisher;
            set { selectedPublisher = value; OnPropertyChanged(); }
        }

        public ObservableCollection<LookupItem> Authors { get; set; }

        public LookupItem SelectedAuthor
        {
            get => selectedAuthor;
            set { selectedAuthor = value; OnPropertyChanged(); }
        }

        public DateTime NewReadDate
        {
            get => newReadDate;
            set { newReadDate = value; OnPropertyChanged(); }
        }

        public IEnumerable<int> YearsList { get; set; }

        public int SelectedReleaseYear
        {
            get => selectedReleaseYear;
            set { selectedReleaseYear = value; OnPropertyChanged(); }
        }

        public override BookWrapper SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value ?? throw new ArgumentNullException(nameof(SelectedItem));
                OnPropertyChanged();
            }
        }

        public string NewFormatName
        {
            get => newFormatName;
            set { newFormatName = value; OnPropertyChanged(); }
        }

        public string NewGenreName
        {
            get => newGenreName;
            set { newGenreName = value; OnPropertyChanged(); }
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
            //SetChangeTracker();

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
                Book book = null;

                if (id != default)
                {
                    book = await ((BookRepository)DomainService.Repository).LoadAsync(id).ConfigureAwait(false);
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
                        HasChanges = DomainService.Repository.HasChanges();
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
                    if (SelectedItem.Title == "" || SelectedItem.Title is null)
                        SelectedItem.Title = "Book Title";
                }

                void InitializeSelectedLanguageIfNoneSet()
                {
                    if (SelectedLanguage is null)
                    {
                        if (SelectedItem.Model.Language != null)
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
                }

                void InitializeSelectedPublisherIfNoneSet()
                {
                    if (SelectedPublisher is null)
                    {
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
                }

                void SetBooksSelectedReleaseYear()
                {
                    SelectedReleaseYear = SelectedItem.ReleaseYear == 0
                        ? DateTime.Today.Year
                        : SelectedItem.ReleaseYear;
                }

                void InitializeAllBookFormats()
                {
                    if (SelectedItem.Model.Formats != null && SelectedItem.Model.Formats.Count > 0)
                    {
                        if (!AllBookFormats.Any())
                        {
                            AllBookFormats.Clear();

                            foreach (var item in SelectedItem.Model.Formats)
                            {
                                var newLookupItem = new LookupItem { Id = item.Id, DisplayMember = item.Name };
                                AllBookFormats.Add((newLookupItem, true).ToTuple());
                            }
                        }
                    }
                }

                void InitializeAllBookGenres()
                {
                    if (SelectedItem.Model.Genres != null && SelectedItem.Model.Genres.Count > 0)
                    {
                        if (!AllBookGenres.Any())
                        {
                            AllBookGenres.Clear();

                            foreach (var item in SelectedItem.Model.Genres)
                            {
                                var newLookupItem = new LookupItem { Id = item.Id, DisplayMember = item.Name };
                                AllBookGenres.Add((newLookupItem, true).ToTuple());
                            }
                        }
                    }
                }

                void SetTabTitle()
                    => TabTitle = $"{SelectedItem.Model.Title}";

            }
            catch (Exception ex)
            {
                var dialog = new NotificationViewModel("Exception", ex.Message);
                DialogService.OpenDialog(dialog);

                Logger.Error("Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.Message, ex.StackTrace);
            }
        }

        protected override bool SaveItemCanExecute()
        {
            return (!SelectedItem.HasErrors) && (HasChanges || IsNewItem || LanguageIsDirty || PublisherIsDirty);
        }

        protected override string CreateChangeMessage(DatabaseOperation operation)
        {
            return $"{operation.ToString()}: {SelectedItem.Title}.";
        }

        public override async void SwitchEditableStateExecute()
        {
            base.SwitchEditableStateExecute();

            await InitializeLanguageCollection();
            await InitializePublisherCollection();
            await InitializeAuthorCollection();
            await InitializeAllBookFormatsCollection();
            await InitializeAllBookGenresCollection();

            SelectedItem.ReleaseYear = SelectedReleaseYear == 0 ? 1 : SelectedReleaseYear;

            async Task InitializeLanguageCollection()
            {
                if (!Languages.Any())
                {
                    Languages.Clear();

                    foreach (var item in await GetLanguageList())
                    {
                        Languages.Add(item);
                    }

                    if (SelectedItem.Model.Language != null)
                        SelectedLanguage = Languages.SingleOrDefault(l => l.Id == SelectedItem.Model.Language.Id);
                }
            }

            async Task InitializePublisherCollection()
            {
                if (!Publishers.Any())
                {
                    Publishers.Clear();

                    foreach (var item in await GetPublisherList())
                    {
                        Publishers.Add(item);
                    }

                    if (SelectedItem.Model.Publisher != null)
                        SelectedPublisher = Publishers.SingleOrDefault(p => p.Id == SelectedItem.Model.Publisher.Id);
                }
            }

            async Task InitializeAuthorCollection()
            {
                if (!Authors.Any())
                {
                    Authors.Clear();

                    foreach (var item in await GetAuthorList())
                    {
                        Authors.Add(item);
                    }
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
                    if (item.Item2 == true)
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
                    if (item.Item2 == true)
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

        private async Task<IEnumerable<LookupItem>> GetPublisherList()
            => await (DomainService as BookService)!.GetPublisherLookupAsync(nameof(PublisherDetailViewModel))
                .ConfigureAwait(false);

        private async Task<IEnumerable<LookupItem>> GetLanguageList()
            => await (DomainService as BookService)!.GetLanguageLookupAsync(nameof(LanguageDetailViewModel))
                .ConfigureAwait(false);

        private async Task<IEnumerable<LookupItem>> GetAuthorList()
            => await (DomainService as BookService)!.GetAuthorLookupAsync(nameof(AuthorDetailViewModel))
                .ConfigureAwait(false);

        private async Task<IEnumerable<LookupItem>> GetBookFormatList()
            => await (DomainService as BookService)!.GetFormatLookupAsync(nameof(FormatDetailViewModel));

        private async Task<IEnumerable<LookupItem>> GetBookGenreList()
            => await (DomainService as BookService)!.GetGenreLookupAsync(nameof(GenreDetailViewModel));

        private async void RemoveAuthorExecute(Guid? authorId)
        {
            if (authorId != null)
            {
                var removedAuthor = await ((IBookRepository) DomainService.Repository).GetAuthorAsync((Guid)authorId);
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

                //SetChangeTracker();
            }
        }

        private async void AddBookAuthorExecute(LookupItem lookupItem)
        {
            if (lookupItem is null) return;

            var addedAuthor = await ((IBookRepository) DomainService.Repository).GetAuthorAsync(lookupItem.Id);

            SelectedItem.Model.Authors.Add(addedAuthor);

            Authors.Remove(lookupItem);

            //SetChangeTracker();
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

        private async void OnBookFormatSelectionChangedExecute(LookupItem lookupItem)
        {
            if (AllBookFormats.Any(bf => bf.Item1.Id == lookupItem.Id && bf.Item2 == false))
            {
                AllBookFormats.Remove(AllBookFormats.Single(bf => bf.Item1.Id == lookupItem.Id));
                AllBookFormats.Add((lookupItem, true).ToTuple());

                var newFormat = await ((IBookRepository) DomainService.Repository).GetFormatAsync(lookupItem.Id);

                SelectedItem.Model.Formats.Add(newFormat);
            }
            else if (AllBookFormats.Any(bf => bf.Item1.Id == lookupItem.Id && bf.Item2 == true))
            {
                AllBookFormats.Remove(AllBookFormats.Single(bf => bf.Item1.Id == lookupItem.Id));
                AllBookFormats.Add((lookupItem, false).ToTuple());

                var item = SelectedItem.Model.Formats.Single(f => f.Id == lookupItem.Id);
                SelectedItem.Model.Formats.Remove(item);
            }
            //SetChangeTracker();
        }

        private async void OnBookGenreSelectionChangedExecute(LookupItem lookupItem)
        {
            if (AllBookGenres.Any(bg => bg.Item1.Id == lookupItem.Id && bg.Item2 == false))
            {
                AllBookGenres.Remove(AllBookGenres.Single(bg => bg.Item1.Id == lookupItem.Id));
                AllBookGenres.Add((lookupItem, true).ToTuple());

                var newGenre = await ((IBookRepository) DomainService.Repository).GetGenreAsync(lookupItem.Id);

                SelectedItem.Model.Genres.Add(newGenre);
            }
            else if (AllBookGenres.Any(bg => bg.Item1.Id == lookupItem.Id && bg.Item2 == true))
            {
                AllBookGenres.Remove(AllBookGenres.Single(bg => bg.Item1.Id == lookupItem.Id));
                AllBookGenres.Add((lookupItem, false).ToTuple());

                var item = SelectedItem.Model.Genres.Single(g => g.Id == lookupItem.Id);
                SelectedItem.Model.Genres.Remove(item);
            }
            //SetChangeTracker();
        }

        private bool OnShowSelectedAuthorCanExecute(Guid? id)
            => (id is null || id == Guid.Empty) ? false : true;

        private void OnShowSelectedAuthorExecute(Guid? id)
            => SelectedAuthorId = (Guid)id;

        private bool OnShowSelectedPublisherCanExecute(Guid? id)
            => (id is null || id == Guid.Empty) ? false : true;

        private void OnShowSelectedPublisherExecute(Guid? id)
            => SelectedPublisherId = (Guid)id;

        //private void OnShowSelectedSeriesExecute(Guid? id)
        //    => SelectedSeriesId = (Guid)id;

        private bool OnShowSelectedSeriesCanExecute(Guid? id)
            => (id is null || id == Guid.Empty) ? false : true;

        private void OnRemoveDateAsABookReadDateExecute(DateTime? readDate)
        {
            if (SelectedItem.Model.ReadDates.Any(d => d.ReadDate == readDate))
            {
                var deletedReadDate = SelectedItem.Model.ReadDates.First(rd => rd.ReadDate == readDate);
                SelectedItem.Model.ReadDates.Remove(deletedReadDate);

                //SetChangeTracker();
            }
        }

        private void OnReleaseYearSelectionChangedExecute()
            => SelectedItem.ReleaseYear = SelectedReleaseYear;

        private IEnumerable<int> PopulateYearsMenu()
        {
            for (int year = DateTime.Today.Year; year > 0; year--)
                yield return year;
        }
        private void OnAddNewAuthorExecute()
        {
            EventAggregator.GetEvent<OpenDetailViewEvent>()
                           .Publish(new OpenDetailViewEventArgs
                           {
                               Id = new Guid(),
                               ViewModelName = nameof(AuthorDetailViewModel)
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

        public override BookWrapper CreateWrapper(Book entity)
        {
            BookWrapper wrapper = new BookWrapper(entity);
            return wrapper;
        }

        private bool OnAddNewFormatCanExecute(string formatName)
        {
            if (formatName is null || formatName == "")
            {
                return false;
            }
            return !AllBookFormats.Any(f 
                => f.Item1.DisplayMember.Equals(formatName, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool OnAddNewGenreCanExecute(string genreName)
        {
            if (genreName is null || genreName == "")
            {
                return false;
            }
            return !AllBookGenres.Any(f 
                => f.Item1.DisplayMember.Equals(genreName, StringComparison.InvariantCultureIgnoreCase));
        }

        private void OnAddNewGenreExecute(string genre)
        {
            AddNewGenre(genre);
        }

        private async Task AddNewGenre(string genre)
        {
            // TODO: 
            var newGenre = Genre.NewGenre;
            //newGenre.SetName(genre);

            await (DomainService as BookService)!.AddNewBookGenre(genre);
            await InitializeAllBookGenresCollection();
        }

        private void OnAddNewFormatExecute(string format)
        {
            AddNewFormat(format);
        }

        private async Task AddNewFormat(string format)
        {
            // TODO: 
            var newFormat = Format.NewFormat;
            newFormat.SetName(format);

            //await (DomainService as BookService).AddNewBookFormat(newFormat);

            await InitializeAllBookFormatsCollection();
        }

    }
}
