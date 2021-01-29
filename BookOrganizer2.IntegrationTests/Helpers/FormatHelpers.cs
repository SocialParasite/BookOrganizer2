using BookOrganizer2.DA.Repositories;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.Shared;
using System.Threading.Tasks;
using Commands = BookOrganizer2.Domain.BookProfile.FormatProfile.Commands;

namespace BookOrganizer2.IntegrationTests.Helpers
{
    public static class FormatHelpers
    {
        public static async Task<Format> CreateValidFormat()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new FormatRepository(context);

            var formatService = new FormatService(repository);

            var command = new Commands.Create
            {
                Id = new FormatId(SequentialGuid.NewSequentialGuid()),
                Name = "paperback"
            };

            await formatService.Handle(command);
            return await repository.GetAsync(command.Id);
        }

        internal static Task UpdateFormat(Format sut)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new FormatRepository(context);

            var formatService = new FormatService(repository);
            var command = new Commands.Update
            {
                Id = sut.Id,
                Name = sut.Name
            };

            return formatService.Handle(command);
        }

        public static Task CreateInvalidFormat()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new FormatRepository(context);
            var formatService = new FormatService(repository);

            var formatId = new FormatId(SequentialGuid.NewSequentialGuid());
            var command = new Commands.Create { Id = formatId };

            return formatService.Handle(command);
        }

        // DELETE
        public static Task RemoveFormat(FormatId id)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new FormatRepository(context);

            var formatService = new FormatService(repository);
            var command = new Commands.Delete
            {
                Id = id,
            };

            return formatService.Handle(command);
        }
    }
}
