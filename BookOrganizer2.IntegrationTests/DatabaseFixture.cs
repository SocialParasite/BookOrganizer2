using BookOrganizer2.DA.SqlServer;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace BookOrganizer2.IntegrationTests
{
    public sealed class DatabaseFixture : IDisposable
    {
        internal BookOrganizer2DbContext context;
        internal string connectionString;

        public DatabaseFixture()
        {
            connectionString = ConnectivityService.GetConnectionString("TEMP");
            context = new BookOrganizer2DbContext(connectionString);
            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            context.Database.ExecuteSqlRaw("DELETE FROM Authors");
        }
    }

    [Trait("Integration", "Database")]
    public sealed partial class DatabaseTests : IClassFixture<DatabaseFixture>
    {
        DatabaseFixture _fixture;

        public DatabaseTests(DatabaseFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }
    }
}