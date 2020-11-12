using BookOrganizer2.Domain.BookProfile.FormatProfile;
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
    public class FormatDetailViewModel : BaseDetailViewModel<Format, FormatId, FormatWrapper>
    {
        private FormatWrapper _selectedItem;
        private readonly IFormatLookupDataService _formatLookupDataService;

        public FormatDetailViewModel(IEventAggregator eventAggregator,
            ILogger logger,
            IDomainService<Format, FormatId> domainService,
            IFormatLookupDataService formatLookupDataService,
            IDialogService dialogService)
            : base(eventAggregator, logger, domainService, dialogService)
        {
            _formatLookupDataService = formatLookupDataService ?? throw new ArgumentNullException(nameof(formatLookupDataService));

            ChangeEditedFormatCommand = new DelegateCommand<Guid?>(OnChangeEditedFormatExecute);
            SaveItemCommand = new DelegateCommand(SaveItemExecute, base.SaveItemCanExecute)
                .ObservesProperty(() => SelectedItem.Name);

            SelectedItem = CreateWrapper(domainService.CreateItem());

            Formats = new ObservableCollection<LookupItem>();

            UserMode = (!UserMode.Item1, DetailViewState.EditMode, Brushes.LightGray, !UserMode.Item4).ToTuple();
        }

        public ICommand ChangeEditedFormatCommand { get; }

        public sealed override FormatWrapper SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value ?? throw new ArgumentNullException(nameof(SelectedItem));
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LookupItem> Formats { get; }

        protected override string CreateChangeMessage(DatabaseOperation operation) 
            => $"{operation}: {SelectedItem.Name}.";

        public sealed override FormatWrapper CreateWrapper(Format entity) => new FormatWrapper(entity);

        public override async Task LoadAsync(Guid id)
        {
            try
            {
                Format format = null;

                if (id != default)
                {
                    format = await DomainService.Repository.GetAsync(id) ?? Format.NewFormat;
                }
                else
                {
                    format = Format.NewFormat;
                    IsNewItem = true;
                }

                SelectedItem = CreateWrapper(format);

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
                }

                await InitializeFormatCollection();

                async Task InitializeFormatCollection()
                {
                    if (!Formats.Any() || HasChanges)
                    {
                        Formats.Clear();

                        foreach (var item in await GetFormatList())
                        {
                            Formats.Add(item);
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

        protected override async void SaveItemExecute()
        {
            base.SaveItemExecute();
            await LoadAsync(SelectedItem.Id);
            NewItemAdded();
        }

        private async Task<IEnumerable<LookupItem>> GetFormatList()
            => await _formatLookupDataService.GetFormatLookupAsync(nameof(FormatDetailViewModel));

        private async void OnChangeEditedFormatExecute(Guid? formatId)
        {
            if (DomainService.Repository.HasChanges())
            {
                var dialog = new OkCancelViewModel("Close the view?", "You have made changes. Changing editable format will loose all unsaved changes. Are you sure you still want to switch?");
                var result = DialogService.OpenDialog(dialog);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            DomainService.Repository.ResetTracking(SelectedItem.Model);
            HasChanges = DomainService.Repository.HasChanges();

            if (formatId is not null) await LoadAsync((Guid) formatId);
        }

        private void NewItemAdded()
        {
            EventAggregator.GetEvent<NewItemEvent>()
                .Publish(new NewItemEventArgs());
        }
    }
}
