using BookOrganizer2.Domain.DA;
using BookOrganizer2.UI.Wpf.Events;
using BookOrganizer2.UI.Wpf.Interfaces;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;
using JetBrains.Annotations;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class MainPageViewModel : ISelectedViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly INationalityLookupDataService _nationalityLookupDataService;
        private readonly IFormatLookupDataService _formatLookupDataService;
        private readonly IGenreLookupDataService _genreLookupDataService;
        private readonly ILanguageLookupDataService _languageLookupDataService;

        public MainPageViewModel([NotNull] IEventAggregator eventAggregator,
                                 [NotNull] INationalityLookupDataService nationalityLookupDataService,
                                 [NotNull] IFormatLookupDataService formatLookupDataService,
                                 [NotNull] IGenreLookupDataService genreLookupDataService,
                                 [NotNull] ILanguageLookupDataService languageLookupDataService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this._nationalityLookupDataService = nationalityLookupDataService 
                                                ?? throw new ArgumentNullException(nameof(nationalityLookupDataService));
            _formatLookupDataService = formatLookupDataService ?? throw new ArgumentNullException(nameof(formatLookupDataService));
            _genreLookupDataService = genreLookupDataService ?? throw new ArgumentNullException(nameof(genreLookupDataService));
            _languageLookupDataService = languageLookupDataService ?? throw new ArgumentNullException(nameof(languageLookupDataService));

            ShowItemsCommand = new DelegateCommand<Type>(OnShowItemsExecute);

            AddNewItemCommand = new DelegateCommand<Type>(OnAddNewItemExecute);
            EditLanguagesCommand = new DelegateCommand(OnEditLanguagesExecute);
            EditNationalitiesCommand = new DelegateCommand(OnEditNationalitiesExecute);
            EditBookFormatsCommand = new DelegateCommand(OnEditBookFormatsExecute);
            EditBookGenresCommand = new DelegateCommand(OnEditBookGenresExecute);
        }

        public ICommand ShowItemsCommand { get; }
        public ICommand AddNewItemCommand { get; }
        public ICommand EditLanguagesCommand { get; }
        public ICommand EditNationalitiesCommand { get; }
        public ICommand EditBookFormatsCommand { get; }
        public ICommand EditBookGenresCommand { get; }

        private void OnShowItemsExecute(Type itemType)
        {
            _eventAggregator.GetEvent<OpenItemViewEvent>()
                           .Publish(new OpenItemViewEventArgs
                           {
                               ViewModelName = Type.GetType(itemType.FullName).Name
                           });
        }

        private void OnAddNewItemExecute(Type itemType)
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                       .Publish(new OpenDetailViewEventArgs
                       {
                           Id = new Guid(),
                           ViewModelName = Type.GetType(itemType.FullName).Name
                       });
        }

        private async void OnEditLanguagesExecute()
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                           .Publish(new OpenDetailViewEventArgs
                           {
                               Id = await _languageLookupDataService.GetLanguageId(),
                               ViewModelName = nameof(LanguageDetailViewModel)
                           });
        }
        private async void OnEditNationalitiesExecute()
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                        .Publish(new OpenDetailViewEventArgs
                        {
                            Id = await _nationalityLookupDataService.GetNationalityId(),
                            ViewModelName = nameof(NationalityDetailViewModel)
                        });
        }

        private async void OnEditBookFormatsExecute()
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                           .Publish(new OpenDetailViewEventArgs
                           {
                               Id = await _formatLookupDataService.GetFormatId(),
                               ViewModelName = nameof(FormatDetailViewModel)
                           });
        }

        private async void OnEditBookGenresExecute()
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                           .Publish(new OpenDetailViewEventArgs
                           {
                               Id = await _genreLookupDataService.GetGenreId(),
                               ViewModelName = nameof(GenreDetailViewModel)
                           });

        }
    }
}
