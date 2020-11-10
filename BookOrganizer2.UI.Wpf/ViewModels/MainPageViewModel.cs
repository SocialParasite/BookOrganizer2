using BookOrganizer2.Domain.DA;
using BookOrganizer2.UI.Wpf.Events;
using BookOrganizer2.UI.Wpf.Interfaces;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class MainPageViewModel : ISelectedViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        //private readonly ILanguageLookupDataService languageLookup;
        private readonly INationalityLookupDataService nationalityLookupDataService;
        //private readonly IFormatLookupDataService formatLookupDataService;
        //private readonly IGenreLookupDataService genreLookupDataService;

        public MainPageViewModel(IEventAggregator eventAggregator,
            INationalityLookupDataService nationalityLookupDataService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.nationalityLookupDataService = nationalityLookupDataService 
                                                ?? throw new ArgumentNullException(nameof(nationalityLookupDataService));

            ShowItemsCommand = new DelegateCommand<Type>(OnShowItemsExecute);

            AddNewItemCommand = new DelegateCommand<Type>(OnAddNewItemExecute);
            //EditLanguagesCommand = new DelegateCommand(OnEditLanguagesExecute);
            EditNationalitiesCommand = new DelegateCommand(OnEditNationalitiesExecute);
            //EditBookFormatsCommand = new DelegateCommand(OnEditBookFormatsExecute);
            //EditBookGenresCommand = new DelegateCommand(OnEditBookGenresExecute);
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

        //private async void OnEditLanguagesExecute()
        //{
        //    eventAggregator.GetEvent<OpenDetailViewEvent>()
        //                   .Publish(new OpenDetailViewEventArgs
        //                   {
        //                       Id = await languageLookup.GetLanguageId(),
        //                       ViewModelName = nameof(LanguageDetailViewModel)
        //                   });
        //}
        private async void OnEditNationalitiesExecute()
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                        .Publish(new OpenDetailViewEventArgs
                        {
                            Id = await nationalityLookupDataService.GetNationalityId(),
                            ViewModelName = nameof(NationalityDetailViewModel)
                        });
        }

        //private async void OnEditBookFormatsExecute()
        //{
        //    eventAggregator.GetEvent<OpenDetailViewEvent>()
        //                   .Publish(new OpenDetailViewEventArgs
        //                   {
        //                       Id = await formatLookupDataService.GetFormatId(),
        //                       ViewModelName = nameof(FormatDetailViewModel)
        //                   });
        //}

        //private async void OnEditBookGenresExecute()
        //{
        //    eventAggregator.GetEvent<OpenDetailViewEvent>()
        //                   .Publish(new OpenDetailViewEventArgs
        //                   {
        //                       Id = await genreLookupDataService.GetGenreId(),
        //                       ViewModelName = nameof(GenreDetailViewModel)
        //                   });

        //}
    }
}
