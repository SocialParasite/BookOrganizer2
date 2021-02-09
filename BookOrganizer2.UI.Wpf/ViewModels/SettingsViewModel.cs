using BookOrganizer2.UI.Wpf.DA;
using BookOrganizer2.UI.Wpf.Enums;
using BookOrganizer2.UI.Wpf.Events;
using BookOrganizer2.UI.Wpf.Interfaces;
using BookOrganizer2.UI.Wpf.Startup;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace BookOrganizer2.UI.Wpf.ViewModels
{
    public class SettingsViewModel : ViewModelBase, ISelectedViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private FileAction _fileActionMode;

        private Settings _settings;
        private Settings _unmodifiedSettings;

        public SettingsViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            ApplyAndSaveSettingsCommand = new DelegateCommand(OnApplyAndSaveExecute, OnApplyAndSaveCanExecute)
                .ObservesProperty(() => HasChanges);
            RemoveConnectionStringCommand = new DelegateCommand<string>(OnRemoveConnectionStringExecute);

            Databases ??= new ObservableCollection<ConnectionString>();

            InitializeRepository();

            HasChanges = true;
        }

        public ICommand ApplyAndSaveSettingsCommand { get; }
        public ICommand RemoveConnectionStringCommand { get; }

        public ObservableCollection<ConnectionString> Databases { get; set; }

        private bool _hasChanges;

        public bool HasChanges
        {
            get => _hasChanges;
            set { _hasChanges = value; OnPropertyChanged(); }
        }

        public string StoragePath
        {
            get => _settings.StoragePath;
            set { _settings.StoragePath = value; OnPropertyChanged(); }
        }

        public FileAction FileActionMode
        {
            get => _fileActionMode;
            set { _fileActionMode = value; OnPropertyChanged(); }
        }

        public string LogFilePath
        {
            get => _settings.LogFilePath;
            set { _settings.LogFilePath = value; OnPropertyChanged(); }
        }

        public string LogServerUrl
        {
            get => _settings.LogServerUrl;
            set { _settings.LogServerUrl = value; OnPropertyChanged(); }
        }

        private void InitializeRepository()
        {
            ReadSettings();

            ReadAndConvertConnectionStrings();
        }

        private void ReadSettings()
        {
            if (!File.Exists(@"Startup\settings.json")) return;

            var settingsFile = File.ReadAllText(@"Startup\settings.json");

            var jobj = (JObject)JsonConvert.DeserializeObject(settingsFile);
            _settings = jobj.ToObject<Settings>();
            _unmodifiedSettings = jobj.ToObject<Settings>();
        }

        private void ReadAndConvertConnectionStrings()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("connectionStrings.json");
            IConfiguration connectionStringConfiguration = builder.Build();

            Databases.Clear();
            IConfigurationSection loop = connectionStringConfiguration.GetSection("ConnectionStrings");

            foreach (var item in loop.GetChildren())
            {
                string newConnectionString = builder.Build().GetConnectionString(item.Key);
                var decoder = new SqlConnectionStringBuilder(newConnectionString);

                var tempConnectionString = new ConnectionString();

                if (item.Key != null && item.Key == _settings.StartupDatabase)
                {
                    tempConnectionString.Default = true;
                }
                tempConnectionString.Database = decoder.InitialCatalog;
                tempConnectionString.Identifier = item.Key;
                tempConnectionString.Server = decoder.DataSource;
                tempConnectionString.Trusted_Connection = decoder.IntegratedSecurity;

                Databases.Add(tempConnectionString);
            }
        }

        private bool OnApplyAndSaveCanExecute() => HasChanges;

        private void OnApplyAndSaveExecute()
        {
            SaveSettingsJson();
            SaveConnectionStrings();
        }

        private void SaveConnectionStrings()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("  \"ConnectionStrings\": {");

            foreach (var db in Databases)
            {
                if (db.Identifier is null || db.Server is null || db.Database is null)
                {
                    continue;
                }

                var builder = new SqlConnectionStringBuilder
                {
                    ["Server"] = db.Server,
                    ["Trusted_Connection"] = db.Trusted_Connection,
                    ["Database"] = db.Database
                };

                stringBuilder.AppendLine($"    \"{db.Identifier}\": \"{builder}\",");
            }

            stringBuilder.AppendLine("  }");
            stringBuilder.AppendLine("}");
            stringBuilder.Replace(@"\", @"\\");

            File.WriteAllText("connectionStrings.json", stringBuilder.ToString());
        }

        private void SaveSettingsJson()
        {
            _settings.StartupDatabase = Databases.Where(d => d.Default == true).Select(d => d.Identifier).FirstOrDefault();

            dynamic jsonSettings = JsonConvert.SerializeObject(_settings, Formatting.Indented);
            File.WriteAllText(@"Startup\settings.json", jsonSettings);

            _eventAggregator.GetEvent<ChangeDetailsViewEvent>()
                    .Publish(new ChangeDetailsViewEventArgs
                    {
                        Message = CreateChangeMessage(DatabaseOperation.SETTINGS),
                        MessageBackgroundColor = Brushes.Green
                    });
        }

        private void OnRemoveConnectionStringExecute(string id)
        {
            if (id != null || id != Databases.LastOrDefault()?.Identifier)
            {
                Databases.Remove(Databases.First(i => i.Identifier == id));

                _eventAggregator.GetEvent<ChangeDetailsViewEvent>()
                    .Publish(new ChangeDetailsViewEventArgs
                    {
                        Message = CreateChangeMessage(DatabaseOperation.DATABASE_CONNECTIONS),
                        MessageBackgroundColor = Brushes.Red
                    });
            }
        }

        private string CreateChangeMessage(DatabaseOperation operation)
            => $"{operation} modified.";
    }
}
