using BookOrganizer2.DA.Repositories;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.Shared;
using System;
using System.Threading.Tasks;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
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

        internal static async Task UpdateNationality(Nationality sut)
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

            await nationalityService.Handle(command);
        }

        public static async Task CreateInvalidNationality()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new NationalityRepository(context);
            var nationalityService = new NationalityService(repository);

            var nationalityId = new NationalityId(SequentialGuid.NewSequentialGuid());
            var command = new Commands.Create { Id = nationalityId };

            await nationalityService.Handle(command);
        }

        // DELETE
        public static async Task RemoveNationality(NationalityId id)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new NationalityRepository(context);

            var nationalityService = new NationalityService(repository);
            var command = new Commands.DeleteNationality
            {
                Id = id,
            };

            await nationalityService.Handle(command);
        }
    }
}
