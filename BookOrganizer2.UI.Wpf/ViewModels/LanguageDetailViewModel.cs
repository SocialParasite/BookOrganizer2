using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.Enums;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using BookOrganizer2.UI.Wpf.Enums;
using BookOrganizer2.UI.Wpf.Wrappers;
using Prism.Commands;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using BookOrganizer2.UI.Wpf.Events;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class LanguageDetailViewModel : BaseDetailViewModel<Language, LanguageId, LanguageWrapper>
    {
        private LanguageWrapper _selectedItem;
        private readonly ILanguageLookupDataService _languageLookupDataService;

        public LanguageDetailViewModel(IEventAggregator eventAggregator,
            ILogger logger,
            ISimpleDomainService<Language, LanguageId> domainService,
            ILanguageLookupDataService languageLookupDataService,
            IDialogService dialogService)
            : base(eventAggregator, logger, domainService, dialogService)
        {
            _languageLookupDataService = languageLookupDataService ?? throw new ArgumentNullException(nameof(languageLookupDataService));

            ChangeEditedLanguageCommand = new DelegateCommand<Guid?>(OnChangeEditedLanguageExecute);
            SaveItemCommand = new DelegateCommand(SaveItemExecute, base.SaveItemCanExecute)
                .ObservesProperty(() => SelectedItem.Name);

            SelectedItem = CreateWrapper(domainService.CreateItem());

            Languages = new ObservableCollection<LookupItem>();

            UserMode = (!UserMode.Item1, DetailViewState.EditMode, Brushes.LightGray, !UserMode.Item4).ToTuple();
        }

        public ICommand ChangeEditedLanguageCommand { get; }

        public sealed override LanguageWrapper SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value ?? throw new ArgumentNullException(nameof(SelectedItem));
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LookupItem> Languages { get; }

        protected override string CreateChangeMessage(DatabaseOperation operation) 
            => $"{operation}: {SelectedItem.Name}.";

        public sealed override LanguageWrapper CreateWrapper(Language entity) => new LanguageWrapper(entity);

        public override async Task LoadAsync(Guid id)
        {
            try
            {
                Language language = null;

                if (id != default)
                {
                    language = await ((ISimpleDomainService<Language, LanguageId>)DomainService).GetAsync(id) ?? Language.NewLanguage;
                }
                else
                {
                    language = Language.NewLanguage;
                    IsNewItem = true;
                }

                SelectedItem = CreateWrapper(language);

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
                    if (e.PropertyName == nameof(SelectedItem.Name)
                        || e.PropertyName == nameof(SelectedItem.Name))
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
                    SelectedItem.Name = SelectedItem.Model.Name;
                    Id = language.Id;
                }

                await InitializeLanguageCollection();

                async Task InitializeLanguageCollection()
                {
                    if (!Languages.Any() || HasChanges)
                    {
                        Languages.Clear();

                        foreach (var item in await GetLanguageList())
                        {
                            Languages.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationViewModel("Exception", ex.Message);
                DialogService.OpenDialog(dialog);

                Logger.Error("Message: {Message}\n\n Stack trace: {StackTrace}\n\n", ex.Message, ex.StackTrace);
            }
        }

        protected override void SaveItemExecute()
        {
            SaveItem().Await();
        }

        private async Task SaveItem()
        {
            base.SaveItemExecute();
            await LoadAsync(SelectedItem.Id);
            NewItemAdded();
        }

        private Task<IEnumerable<LookupItem>> GetLanguageList()
            => _languageLookupDataService.GetLanguageLookupAsync(nameof(LanguageDetailViewModel));

        private async void OnChangeEditedLanguageExecute(Guid? languageId)
        {
            if (DomainService.HasChanges())
            {
                var dialog = new OkCancelViewModel("Close the view?", "You have made changes. Changing editable language will loose all unsaved changes. Are you sure you still want to switch?");
                var result = DialogService.OpenDialog(dialog);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            ((ISimpleDomainService<Language, LanguageId>)DomainService).ResetTracking(SelectedItem.Model);
            HasChanges = DomainService.HasChanges();

            if (languageId is not null) await LoadAsync((Guid) languageId);
        }

        private void NewItemAdded()
        {
            EventAggregator.GetEvent<NewItemEvent>()
                .Publish(new NewItemEventArgs());
        }
    }
}
