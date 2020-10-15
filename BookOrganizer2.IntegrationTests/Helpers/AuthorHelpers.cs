using BookOrganizer2.DA.Repositories;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using System;
using System.Threading.Tasks;

namespace BookOrganizer2.IntegrationTests.Helpers
{
    public static class AuthorHelpers
    {
        public static async Task<Author> CreateValidAuthor()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new AuthorRepository(context);

            var authorService = new AuthorService(repository);
            var authorId = new AuthorId(SequentialGuid.NewSequentialGuid());
            Author author = Author.Create(authorId);
            author.SetFirstName("Patrick");
            author.SetLastName("Rothfuss");
            author.SetDateOfBirth(new DateTime(1973, 6, 6));
            author.SetMugshotPath(@"\\filepath\file.jpg");
            author.SetBiography("There is no book number three.");

            await authorService.HandleCreate(author);

            return await repository.GetAsync(author.Id);
        }
    }
}
