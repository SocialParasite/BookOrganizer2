using BookOrganizer2.DA.Repositories;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.Domain.Shared;
using System;
using System.Threading.Tasks;
using Commands = BookOrganizer2.Domain.PublisherProfile.Commands;

namespace BookOrganizer2.IntegrationTests.Helpers
{
    public static class PublisherHelpers
    {
        public static async Task<Publisher> CreateValidPublisher()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new PublisherRepository(context);

            var publisherService = new PublisherService(repository);

            var command = new Commands.Create
            {
                Id = new PublisherId(SequentialGuid.NewSequentialGuid()),
                Name = "SubPop",
                LogoPath = @"\\filepath\file.jpg",
                Description = "Best books in the world!"
            };

            await publisherService.Handle(command);
            return await repository.GetAsync(command.Id);
        }

        internal static Task UpdatePublisher(Publisher sut)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new PublisherRepository(context);

            var publisherService = new PublisherService(repository);
            var command = new Commands.Update
            {
                Id = sut.Id,
                Name = sut.Name,
                LogoPath = sut.LogoPath,
                Description = sut.Description
            };

            return publisherService.Handle(command);
        }

        public static Task CreateInvalidPublisher()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new PublisherRepository(context);
            var publisherService = new PublisherService(repository);

            var publisherId = new PublisherId(SequentialGuid.NewSequentialGuid());
            var command = new Commands.Create { Id = publisherId };

            return publisherService.Handle(command);
        }

        public static Task UpdatePublisherName(PublisherId id, string name)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new PublisherRepository(context);

            var publisherService = new PublisherService(repository);
            var command = new Commands.SetName
            {
                Id = id,
                Name = name
            };

            return publisherService.Handle(command);
        }

        public static Task UpdatePublisherLogoPath(PublisherId id, string path)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new PublisherRepository(context);

            var publisherService = new PublisherService(repository);
            var command = new Commands.SetLogoPath
            {
                Id = id,
                LogoPath = path
            };

            return publisherService.Handle(command);
        }

        public static Task UpdatePublisherDescription(PublisherId id, string description)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new PublisherRepository(context);

            var publisherService = new PublisherService(repository);
            var command = new Commands.SetDescription()
            {
                Id = id,
                Description = description
            };

            return publisherService.Handle(command);
        }

        // DELETE
        public static Task RemovePublisher(PublisherId id)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new PublisherRepository(context);

            var publisherService = new PublisherService(repository);
            var command = new Commands.DeletePublisher
            {
                Id = id,
            };

            return publisherService.Handle(command);
        }
    }
}
