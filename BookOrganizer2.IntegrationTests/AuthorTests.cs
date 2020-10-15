using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookOrganizer2.Domain;
using Xunit;

namespace BookOrganizer2.IntegrationTests
{
    public sealed partial class DatabaseTests
    {
        [Fact]
        public async Task Author_inserted_to_database()
        {
            Author author = new Author();
            author.SetFirstName("Patrick");
            author.SetLastName("Rothfuss");
            author.SetDateOfBirth(new DateTime(1973, 6, 6));
            author.SetMugshotPath(@"\\filepath\file.jpg");
            author.SetBiography("There is no book number three.");
            //Author author = await CreateValidAuthor();

            //var repository = new AuthorRepository(fixture.context);
            //var sut = await repository.LoadAsync(author.Id);

            //Assert.True(await repository.ExistsAsync(sut.Id));
            //Assert.True(sut.FirstName.Length > 0);
        }
    }
}