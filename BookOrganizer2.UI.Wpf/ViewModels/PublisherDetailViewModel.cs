using BookOrganizer2.DA.Repositories;
using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using BookOrganizer2.UI.Wpf.Enums;
using BookOrganizer2.UI.Wpf.Services;
using BookOrganizer2.UI.Wpf.Wrappers;
using Prism.Commands;
using Prism.Events;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using BookOrganizer2.UI.Wpf.Events;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class PublisherDetailViewModel : BaseDetailViewModel<Publisher, PublisherId, PublisherWrapper>
    {
        private PublisherWrapper _selectedItem;

        public PublisherDetailViewModel(IEventAggregator eventAggregator,
                                     ILogger logger,
                                     IPublisherDomainService domainService,
                                     IDialogService dialogService)
            : base(eventAggregator, logger, domainService, dialogService)
        {
            AddPublisherLogoCommand = new DelegateCommand(OnAddPublisherLogoExecute);
            SaveItemCommand = new DelegateCommand(SaveItemExecute, SaveItemCanExecute)
                .ObservesProperty(() => SelectedItem.Name);

            SelectedItem = new PublisherWrapper(domainService.CreateItem());
        }

        public ICommand AddPublisherLogoCommand { get; }

        public sealed override PublisherWrapper SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value ?? throw new ArgumentNullException(nameof(SelectedItem));
                OnPropertyChanged();
            }
        }

        private void OnAddPublisherLogoExecute()
        {
            SelectedItem.LogoPath = FileExplorerService.BrowsePicture() ?? SelectedItem.LogoPath;
        }

        protected override string CreateChangeMessage(DatabaseOperation operation)
        {
            return $"{operation.ToString()}: {SelectedItem.Name}.";
        }

        public override async Task LoadAsync(Guid id)
        {
            try
            {
                Publisher publisher;

                if (id != default)
                {
                    publisher = await ((IPublisherDomainService)DomainService).LoadAsync(id);
                }
                else
                {
                    publisher = Publisher.NewPublisher;
                    IsNewItem = true;
                }

                SelectedItem = CreateWrapper(publisher);

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
                    Id = publisher.Id;
                }

                SetDefaultPublisherLogoIfNoneSet();

                void SetDefaultPublisherLogoIfNoneSet()
                {
                    SelectedItem.LogoPath ??= FileExplorerService.GetImagePath();
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

            base.SaveItemExecute();
            NewPublisherAdded();
        }

        private void NewPublisherAdded()
        {
            EventAggregator.GetEvent<NewPublisherEvent>()
                .Publish(new NewPublisherEventArgs());
        }

        public override PublisherWrapper CreateWrapper(Publisher entity) => new(entity);
    }
}
