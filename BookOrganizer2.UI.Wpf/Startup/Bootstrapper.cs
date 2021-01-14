using System.IO;
using Autofac;
using BookOrganizer2.DA.Repositories;
using BookOrganizer2.DA.Repositories.Lookups;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.Wpf.Interfaces;
using BookOrganizer2.UI.Wpf.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Events;
using Serilog;

namespace BookOrganizer2.UI.Wpf.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<DialogService>().As<IDialogService>();

            builder.RegisterAssemblyTypes(typeof(AuthorService).Assembly)
                .Where(type => type.Name.EndsWith("Service"))
                .AsClosedTypesOf(typeof(IDomainService<,>));

            builder.RegisterAssemblyTypes(typeof(AuthorsViewModel).Assembly)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .Keyed<ISelectedViewModel>(c => c.Name);

            builder.RegisterAssemblyTypes(typeof(AuthorDetailViewModel).Assembly)
                .Where(type => type.Name.EndsWith("DetailViewModel"))
                .Keyed<IDetailViewModel>(c => c.Name);

            builder.RegisterAssemblyTypes(typeof(AuthorLookupDataService).Assembly)
                .Where(type => type.Name.EndsWith("LookupDataService"))
                .AsImplementedInterfaces()
                .WithParameter("imagePath", "");

            //builder.RegisterType<ReportLookupDataService>().AsImplementedInterfaces();
            //builder.RegisterAssemblyTypes(typeof(AnnualBookStatisticsReportViewModel).Assembly)
            //    .Where(type => type.Name.EndsWith("ReportViewModel"))
            //    .Keyed<IReport>(c => c.Name);

            builder.RegisterAssemblyTypes(typeof(AuthorRepository).Assembly)
                .Where(type => type.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();

            Settings settings = GetSettings();

            builder.Register<ILogger>((_)
                => new LoggerConfiguration()
                    .WriteTo.File(Path.Combine(settings.LogFilePath, "Log-{Date}.txt"), rollingInterval: RollingInterval.Day)
                    .WriteTo.Seq(settings.LogServerUrl)
                    .CreateLogger())
                .SingleInstance();

            var connectionString = ConnectivityService.GetConnectionString(settings.StartupDatabase);

            builder.RegisterType<BookOrganizer2DbContext>().AsSelf().WithParameter("connectionString", connectionString);

            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            builder.RegisterType<MainWindow>().AsSelf().SingleInstance();
            builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();

            return builder.Build();
        }

        Settings GetSettings()
        {
            Settings settings = null;

            if (File.Exists(@"Startup\settings.json"))
            {
                var settingsFile = File.ReadAllText(@"Startup\settings.json");

                JObject jobj = (JObject)JsonConvert.DeserializeObject(settingsFile);
                settings = jobj?.ToObject<Settings>();
            }

            return settings;
        }
    }
}
