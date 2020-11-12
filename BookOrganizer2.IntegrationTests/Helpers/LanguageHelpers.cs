using BookOrganizer2.DA.Repositories;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.Shared;
using System.Threading.Tasks;
using Commands = BookOrganizer2.Domain.BookProfile.LanguageProfile.Commands;

namespace BookOrganizer2.IntegrationTests.Helpers
{
    public static class LanguageHelpers
    {
        public static async Task<Language> CreateValidLanguage()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new LanguageRepository(context);

            var languageService = new LanguageService(repository);

            var command = new Commands.Create
            {
                Id = new LanguageId(SequentialGuid.NewSequentialGuid()),
                Name = "pig latin"
            };

            await languageService.Handle(command);
            return await repository.GetAsync(command.Id);
        }

        internal static Task UpdateLanguage(Language sut)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new LanguageRepository(context);

            var languageService = new LanguageService(repository);
            var command = new Commands.Update
            {
                Id = sut.Id,
                Name = sut.Name
            };

            return languageService.Handle(command);
        }

        public static Task CreateInvalidLanguage()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new LanguageRepository(context);
            var languageService = new LanguageService(repository);

            var languageId = new LanguageId(SequentialGuid.NewSequentialGuid());
            var command = new Commands.Create { Id = languageId };

            return languageService.Handle(command);
        }

        // DELETE
        public static Task RemoveLanguage(LanguageId id)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new LanguageRepository(context);

            var languageService = new LanguageService(repository);
            var command = new Commands.DeleteLanguage
            {
                Id = id,
            };

            return languageService.Handle(command);
        }
    }
}
