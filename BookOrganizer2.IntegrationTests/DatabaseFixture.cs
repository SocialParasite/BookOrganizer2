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
            Context.Database.ExecuteSqlRaw("DELETE FROM BookReadDate");
            Context.Database.ExecuteSqlRaw("DELETE FROM Books");
            Context.Database.ExecuteSqlRaw("DELETE FROM Series");
            Context.Database.ExecuteSqlRaw("DELETE FROM Authors");
            Context.Database.ExecuteSqlRaw("DELETE FROM AuthorBook");
            Context.Database.ExecuteSqlRaw("DELETE FROM BookFormat");
            Context.Database.ExecuteSqlRaw("DELETE FROM BookGenre");
            Context.Database.ExecuteSqlRaw("DELETE FROM Nationalities");
            Context.Database.ExecuteSqlRaw("DELETE FROM Publishers");
            Context.Database.ExecuteSqlRaw("DELETE FROM Formats");
            Context.Database.ExecuteSqlRaw("DELETE FROM Genres");
            Context.Database.ExecuteSqlRaw("DELETE FROM Languages");
        }
    }

    [Trait("Integration", "DB\\_ALL")]
    public sealed partial class DatabaseTests : IClassFixture<DatabaseFixture>
    {
        readonly DatabaseFixture _fixture;

        public DatabaseTests(DatabaseFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }
    }
}