using BookOrganizer2.DA.Repositories;
using BookOrganizer2.IntegrationTests.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace BookOrganizer2.IntegrationTests
{
    public sealed partial class DatabaseTests
    {
        [Fact]
        public async Task Author_inserted_to_database()
        {
            var author = await AuthorHelpers.CreateValidAuthor();
            var repository = new AuthorRepository(_fixture.Context);

            Assert.True(await repository.ExistsAsync(author.Id));
        }
    }
}