using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using System;
using System.Threading.Tasks;
using static BookOrganizer2.Domain.BookProfile.FormatProfile.Commands;

namespace BookOrganizer2.Domain.BookProfile.FormatProfile
{
    public class FormatService : IFormatService
    {
        public IRepository<Format, FormatId> Repository { get; }

        public FormatService(IRepository<Format, FormatId> repository) 
            => Repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public Format CreateItem() 
            => Format.NewFormat;

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

        public async Task<Format> AddNew(Format model)
        {
            var command = new Create
            {
                Id = new FormatId(SequentialGuid.NewSequentialGuid()),
                Name = model.Name
            };

            await Handle(command);

            return await Repository.GetAsync(command.Id);
        }

        public async Task<Format> AddNew(string name)
        {
            var command = new Create
            {
                Id = new FormatId(SequentialGuid.NewSequentialGuid()),
                Name = name
            };

            await Handle(command);

            return await Repository.GetAsync(command.Id);
        }

        public Guid GetId(FormatId id) => id?.Value ?? Guid.Empty;
        public Task Update(Format model)
        {
            var command = new Update
            {
                Id = model.Id,
                Name = model.Name
            };

            return Handle(command);
        }

        public Task RemoveAsync(FormatId id)
        {
            var command = new Delete
            {
                Id = id.Value
            };

            return Handle(command);
        }

        private async Task HandleCreate(Create cmd)
        {
            if (await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var format = Format.Create(cmd.Id, cmd.Name);

            await Repository.AddAsync(format);

            if (format.EnsureValidState())
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

            var updatableFormat = await Repository.GetAsync(cmd.Id);

            updatableFormat.SetName(cmd.Name);
            
            Repository.Update(updatableFormat);

            if (updatableFormat.EnsureValidState())
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

            try
            {
                await Repository.RemoveAsync(cmd.Id);
                await Repository.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(ex.Message);
            }
        }
    }
}
