using Microsoft.Extensions.Configuration;

namespace BookOrganizer2.DA.SqlServer
{
    public sealed class ConnectivityService
    {
        public static string GetConnectionString(string database = "DEV")
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("connectionStrings.json");

            return builder.Build().GetConnectionString(database);
        }
    }
}
