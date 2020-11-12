using BookOrganizer2.DA.Repositories;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.Shared;
using System.Threading.Tasks;
using Commands = BookOrganizer2.Domain.BookProfile.GenreProfile.Commands;

namespace BookOrganizer2.IntegrationTests.Helpers
{
    public static class GenreHelpers
    {
        public static async Task<Genre> CreateValidGenre()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new GenreRepository(context);

            var genreService = new GenreService(repository);

            var command = new Commands.Create
            {
                Id = new GenreId(SequentialGuid.NewSequentialGuid()),
                Name = "sci-fi"
            };

            await genreService.Handle(command);
            return await repository.GetAsync(command.Id);
        }

        internal static Task UpdateGenre(Genre sut)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new GenreRepository(context);

            var genreService = new GenreService(repository);
            var command = new Commands.Update
            {
                Id = sut.Id,
                Name = sut.Name
            };

            return genreService.Handle(command);
        }

        public static Task CreateInvalidGenre()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new GenreRepository(context);
            var genreService = new GenreService(repository);

            var genreId = new GenreId(SequentialGuid.NewSequentialGuid());
            var command = new Commands.Create { Id = genreId };

            return genreService.Handle(command);
        }

        // DELETE
        public static Task RemoveGenre(GenreId id)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new GenreRepository(context);

            var genreService = new GenreService(repository);
            var command = new Commands.DeleteGenre
            {
                Id = id,
            };

            return genreService.Handle(command);
        }
    }
}
