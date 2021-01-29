using BookOrganizer2.DA.Repositories;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using BookOrganizer2.Domain.Shared;
using System.Threading.Tasks;
using Commands = BookOrganizer2.Domain.AuthorProfile.NationalityProfile.Commands;

namespace BookOrganizer2.IntegrationTests.Helpers
{
    public static class NationalityHelpers
    {
        public static async Task<Nationality> CreateValidNationality()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new NationalityRepository(context);

            var nationalityService = new NationalityService(repository);

            var command = new Commands.Create
            {
                Id = new NationalityId(SequentialGuid.NewSequentialGuid()),
                Name = "american"
            };

            await nationalityService.Handle(command);
            return await repository.GetAsync(command.Id);
        }

        internal static Task UpdateNationality(Nationality sut)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new NationalityRepository(context);

            var nationalityService = new NationalityService(repository);
            var command = new Commands.Update
            {
                Id = sut.Id,
                Name = sut.Name
            };

            return nationalityService.Handle(command);
        }

        public static Task CreateInvalidNationality()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new NationalityRepository(context);
            var nationalityService = new NationalityService(repository);

            var nationalityId = new NationalityId(SequentialGuid.NewSequentialGuid());
            var command = new Commands.Create { Id = nationalityId };

            return nationalityService.Handle(command);
        }

        // DELETE
        public static Task RemoveNationality(NationalityId id)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new NationalityRepository(context);

            var nationalityService = new NationalityService(repository);
            var command = new Commands.Delete
            {
                Id = id,
            };

            return nationalityService.Handle(command);
        }
    }
}
