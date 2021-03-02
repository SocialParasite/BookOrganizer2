using Autofac;
using System.Windows;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.Wpf.Startup;

namespace BookOrganizer2.UI.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static IContainer Container { get; set; }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var bootstrapper = new Bootstrapper();

            Container = bootstrapper.Bootstrap();

            var mainWindow = Container.Resolve<MainWindow>();
            mainWindow.Show();

            var dbConnectivity = new DbConnectivityTester(Container.Resolve<IDialogService>(),
                                                          Container.Resolve<BookOrganizer2DbContext>().ConnectionString);
            await dbConnectivity.DbConnectivityCheckAsync();
        }
    }
}
