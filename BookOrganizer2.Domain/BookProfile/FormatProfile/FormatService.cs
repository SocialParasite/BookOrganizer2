using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using System;
using System.Threading.Tasks;
using static BookOrganizer2.Domain.BookProfile.FormatProfile.Commands;

namespace BookOrganizer2.Domain.BookProfile.FormatProfile
{
    public class FormatService : IDomainService<Format, FormatId>
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
                //SetFormatName cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetName(cmd.Name), (a) => Repository.Update(a)),
                DeleteFormat cmd => HandleUpdateAsync(cmd.Id, _ => Repository.RemoveAsync(cmd.Id)),
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

        public Guid GetId(FormatId id) => id?.Value ?? Guid.Empty;

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

        private async Task HandleUpdateAsync(Guid id, Action<Format> operation, Action<Format> operation2 = null)
        {
            if (await Repository.ExistsAsync(id))
            {
                var format = await Repository.GetAsync(id);
                operation(format);
                operation2?.Invoke(format);

                if (format.EnsureValidState())
                {
                    await Repository.SaveAsync();
                }
            }
            else
                throw new ArgumentException();
        }
    }
}
