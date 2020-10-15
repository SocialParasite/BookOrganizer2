using BookOrganizer2.DA.SqlServer;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace BookOrganizer2.IntegrationTests
{
    public sealed class DatabaseFixture : IDisposable
    {
        internal readonly BookOrganizer2DbContext Context;

        public DatabaseFixture()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            Context = new BookOrganizer2DbContext(connectionString);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Database.ExecuteSqlRaw("DELETE FROM Authors");
        }
    }

    [Trait("Integration", "Database")]
    public sealed partial class DatabaseTests : IClassFixture<DatabaseFixture>
    {
        readonly DatabaseFixture _fixture;

        public DatabaseTests(DatabaseFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }
    }
}