using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.Enums;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;

namespace BookOrganizer2.UI.Wpf.Startup
{
    public class DbConnectivityTester
    {
        private readonly IDialogService _dialogService;
        private readonly string _connectionString;

        public DbConnectivityTester(IDialogService dialogService, string connString = null)
        {
            this._dialogService = dialogService;
            _connectionString = connString ?? ConnectivityService.GetConnectionString();
        }

        public async Task DbConnectivityCheckAsync()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = _connectionString;

            try
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

                connection.Open();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cannot open database"))
                {
                    var dialog = new NotificationViewModel("Database problem",
                        ($"Book Organizer database couldn't be opened. " +
                         $"This might be because the database is down or it doesn't exist. \r\rError message(s):\r{ex.Message}"
                        ));

                    _dialogService.OpenDialog(dialog);

                    if (ShouldDatabaseBeCreated())
                    {
                        await CreateDatabase();
                    }
                }
                else
                    _dialogService.OpenDialog(new NotificationViewModel("Error", ex.Message));
            }

            connection.Dispose();
        }

        private bool ShouldDatabaseBeCreated()
        {
            var dialog = new OkCancelViewModel("Would you like to create a database now?", "Create database?");

            return _dialogService.OpenDialog(dialog) == DialogResult.Yes;
        }

        private async Task CreateDatabase()
        {
            var context = new BookOrganizer2DbContext(_connectionString);
            await context.Database.EnsureCreatedAsync();
            Application.Current.MainWindow?.Close();
        }
    }
}
