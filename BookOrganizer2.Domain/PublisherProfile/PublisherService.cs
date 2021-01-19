using System;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using static BookOrganizer2.Domain.PublisherProfile.Commands;

namespace BookOrganizer2.Domain.PublisherProfile
{
    public class PublisherService : IPublisherDomainService
    {
        public readonly INationalityLookupDataService NationalityLookupDataService;
        public IRepository<Publisher, PublisherId> Repository { get; }

        public PublisherService(IRepository<Publisher, PublisherId> repository,
            INationalityLookupDataService nationalityLookupDataService = null)
        {
            NationalityLookupDataService = nationalityLookupDataService;
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Publisher CreateItem()
            => Publisher.NewPublisher;

        public Task Handle(object command)
        {
            return command switch
            {
                Create cmd => HandleCreate(cmd),
                Update cmd => HandleFullUpdate(cmd),
                SetName cmd => HandleUpdate(cmd.Id, (a) => a.SetName(cmd.Name),
                    (a) => Repository.Update(a)),
                SetLogoPath cmd => HandleUpdate(cmd.Id, (a) => a.SetLogoPath(cmd.LogoPath),
                    (a) => Repository.Update(a)),
                SetDescription cmd => HandleUpdate(cmd.Id, (a) => a.SetDescription(cmd.Description),
                    (a) => Repository.Update(a)),
                DeletePublisher cmd => HandleUpdate(cmd.Id, _ => Repository.RemoveAsync(cmd.Id)),
                _ => Task.CompletedTask
            };
        }

        public Guid GetId(PublisherId id) => id?.Value ?? Guid.Empty;

        public async Task<Publisher> AddNew(Publisher model)
        {
            var command = new Create
            {
                Id = new PublisherId(SequentialGuid.NewSequentialGuid()),
                Name = model.Name,
                LogoPath = model.LogoPath,
                Description = model.Description
            };

            await Handle(command);

            return await Repository.GetAsync(command.Id);
        }

        private async Task HandleCreate(Create cmd)
        {
            if (await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var publisher = Publisher.Create(cmd.Id,
                                       cmd.Name,
                                       cmd.LogoPath,
                                       cmd.Description);

            await Repository.AddAsync(publisher);

            if (publisher.EnsureValidState())
            {
                await Repository.SaveAsync();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        private async Task HandleFullUpdate(Update cmd)
        {
            if (!await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} was not found! Update cannot finish.");

            var updatablePublisher = await Repository.GetAsync(cmd.Id);

            updatablePublisher.SetName(cmd.Name);
            updatablePublisher.SetLogoPath(cmd.LogoPath);
            updatablePublisher.SetDescription(cmd.Description);

            Repository.Update(updatablePublisher);

            if (updatablePublisher.EnsureValidState())
            {
                await Repository.SaveAsync();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        private async Task HandleUpdate(Guid id, Action<Publisher> operation, Action<Publisher> operation2 = null)
        {
            if (await Repository.ExistsAsync(id))
            {
                var publisher = await Repository.GetAsync(id);
                operation(publisher);
                operation2?.Invoke(publisher);

                if (publisher.EnsureValidState())
                {
                    await Repository.SaveAsync();
                }
            }
            else
                throw new ArgumentException();
        }

        private async Task HandleUpdateAsync(Guid publisherId, Func<Publisher, Task> operation)
        {
            if (await Repository.ExistsAsync(publisherId))
            {
                var publisher = await Repository.GetAsync(publisherId);

                if (publisher is null)
                    throw new InvalidOperationException($"Entity with id {publisherId} cannot be found");

                await operation(publisher);

                if (publisher.EnsureValidState())
                {
                    await Repository.SaveAsync();
                }
            }
            else
                throw new ArgumentException();
        }
    }
}
