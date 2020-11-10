using BookOrganizer2.DA.Repositories;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using BookOrganizer2.Domain.Shared;
using System;
using System.Threading.Tasks;
using Commands = BookOrganizer2.Domain.AuthorProfile.Commands;

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

            var command = new Commands.Create
            {
                Id = new AuthorId(SequentialGuid.NewSequentialGuid()),
                FirstName = "Patrick",
                LastName = "Rothfuss",
                DateOfBirth = new DateTime(1973, 6, 6),
                MugshotPath = @"\\filepath\file.jpg",
                Biography = "There is no book number three.",
                Notes = "...",
            };

            await authorService.Handle(command);
            return await repository.GetAsync(command.Id);
        }

        public static async Task<Author> CreateValidAuthorWithNationality()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new AuthorRepository(context);

            var authorService = new AuthorService(repository);

            var nationality = await NationalityHelpers.CreateValidNationality();
            var command = new Commands.Create
            {
                Id = new AuthorId(SequentialGuid.NewSequentialGuid()),
                FirstName = "Patrick",
                LastName = "Rothfuss",
                DateOfBirth = new DateTime(1973, 6, 6),
                MugshotPath = @"\\filepath\file.jpg",
                Biography = "There is no book number three.",
                Notes = "...",
                Nationality = nationality
            };

            await authorService.Handle(command);
            return await repository.GetAsync(command.Id);
        }

        internal static Task UpdateAuthor(Author sut)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new AuthorRepository(context);

            var authorService = new AuthorService(repository);
            var command = new Commands.Update
            {
                Id = sut.Id,
                FirstName = sut.FirstName,
                LastName = sut.LastName,
                DateOfBirth = sut.DateOfBirth,
                MugshotPath = sut.MugshotPath,
                Biography = sut.Biography,
                Notes = sut.Notes
            };

            return authorService.Handle(command);
        }

        public static Task CreateInvalidAuthor()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new AuthorRepository(context);
            var authorService = new AuthorService(repository);

            var authorId = new AuthorId(SequentialGuid.NewSequentialGuid());
            var command = new Commands.Create { Id = authorId };

            return authorService.Handle(command);
        }

        public static Task UpdateAuthorFirstName(AuthorId id, string firstName)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new AuthorRepository(context);

            var authorService = new AuthorService(repository);
            var command = new Commands.SetAuthorsFirstName
            {
                Id = id,
                FirstName = firstName
            };

            return authorService.Handle(command);
        }

        public static Task UpdateAuthorLastName(AuthorId id, string lastName)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new AuthorRepository(context);

            var authorService = new AuthorService(repository);
            var command = new Commands.SetAuthorsLastName
            {
                Id = id,
                LastName = lastName
            };

            return authorService.Handle(command);
        }

        public static Task UpdateAuthorDateOfBirth(AuthorId id, DateTime dob)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new AuthorRepository(context);

            var authorService = new AuthorService(repository);
            var command = new Commands.SetAuthorDateOfBirth
            {
                Id = id,
                DataOfBirth = dob
            };

            return authorService.Handle(command);
        }

        public static Task UpdateAuthorMugshotPath(AuthorId id, string path)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new AuthorRepository(context);

            var authorService = new AuthorService(repository);
            var command = new Commands.SetMugshotPath
            {
                Id = id,
                MugshotPath = path
            };

            return authorService.Handle(command);
        }

        public static Task UpdateAuthorBiography(AuthorId id, string bio)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new AuthorRepository(context);

            var authorService = new AuthorService(repository);
            var command = new Commands.SetBiography
            {
                Id = id,
                Biography = bio
            };

            return authorService.Handle(command);
        }

        public static Task UpdateAuthorNotes(AuthorId id, string notes)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new AuthorRepository(context);

            var authorService = new AuthorService(repository);
            var command = new Commands.SetNotes
            {
                Id = id,
                Notes = notes
            };

            return authorService.Handle(command);
        }

        // DELETE
        public static Task RemoveAuthor(AuthorId id)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new AuthorRepository(context);

            var authorService = new AuthorService(repository);
            var command = new Commands.DeleteAuthor
            {
                Id = id,
            };

            return authorService.Handle(command);
        }

        public static Task UpdateAuthorNationality(AuthorId authorId, NationalityId nationalityId)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new AuthorRepository(context);

            var authorService = new AuthorService(repository);
            var command = new Commands.SetNationality
            {
                Id = authorId,
                NationalityId = nationalityId
            };

            return authorService.Handle(command);
        }
    }
}
