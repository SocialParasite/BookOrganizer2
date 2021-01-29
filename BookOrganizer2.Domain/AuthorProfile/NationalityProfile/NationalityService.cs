using System;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using static BookOrganizer2.Domain.AuthorProfile.NationalityProfile.Commands;

namespace BookOrganizer2.Domain.AuthorProfile.NationalityProfile
{
    public class NationalityService : ISimpleDomainService<Nationality, NationalityId>
    {
        public IRepository<Nationality, NationalityId> Repository { get; }

        public NationalityService(IRepository<Nationality, NationalityId> repository) 
            => Repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public Nationality CreateItem()
            => Nationality.NewNationality;

        public Task Handle(object command)
        {
            return command switch
            {
                Create cmd => HandleCreate(cmd),
                Update cmd => HandleUpdate(cmd),
                Delete cmd => HandleDeleteAsync(cmd),
                _ => Task.CompletedTask
            };
        }

        public async Task<Nationality> AddNew(Nationality model)
        {
            var command = new Create
            {
                Id = new NationalityId(SequentialGuid.NewSequentialGuid()),
                Name = model.Name
            };

            await Handle(command);

            return await Repository.GetAsync(command.Id);
        }

        public Guid GetId(NationalityId id) => id?.Value ?? Guid.Empty;
        public Task Update(Nationality model)
        {
            var command = new Events.Updated
            {
                Id = model.Id,
                Name = model.Name
            };

            return Handle(command);
        }

        public Task RemoveAsync(NationalityId id)
        {
            var command = new Delete
            {
                Id = id
            };

            return Handle(command);
        }

        private async Task HandleCreate(Create cmd)
        {
            if (await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var nationality = Nationality.Create(cmd.Id, cmd.Name);

            await Repository.AddAsync(nationality);

            if (nationality.EnsureValidState())
            {
                await Repository.SaveAsync();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        private async Task HandleUpdate(Update cmd)
        {
            if (!await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} was not found! Update cannot finish.");

            var updatableNationality = await Repository.GetAsync(cmd.Id);

            updatableNationality.SetName(cmd.Name);
            
            Repository.Update(updatableNationality);

            if (updatableNationality.EnsureValidState())
            {
                await Repository.SaveAsync();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
        private async Task HandleDeleteAsync(Delete cmd)
        {
            if (!await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} was not found! Update cannot finish.");

            if (Repository.RemoveAsync(cmd.Id) == Task.CompletedTask)
            {
                await Repository.SaveAsync();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
    }
}
