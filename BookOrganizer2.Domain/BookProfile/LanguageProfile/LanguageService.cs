using System;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using static BookOrganizer2.Domain.BookProfile.LanguageProfile.Commands;

namespace BookOrganizer2.Domain.BookProfile.LanguageProfile
{
    public class LanguageService : ISimpleDomainService<Language, LanguageId>
    {
        public IRepository<Language, LanguageId> Repository { get; }

        public LanguageService(IRepository<Language, LanguageId> repository) 
            => Repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public Language CreateItem() 
            => Language.NewLanguage;

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

        public async Task<Language> AddNew(Language model)
        {
            var command = new Create
            {
                Id = new LanguageId(SequentialGuid.NewSequentialGuid()),
                Name = model.Name
            };

            await Handle(command);

            return await Repository.GetAsync(command.Id);
        }

        public Task Update(Language model)
        {
            var command = new Update
            {
                Id = model.Id,
                Name = model.Name
            };

            return Handle(command);
        }

        public Task RemoveAsync(LanguageId id)
        {
            var command = new Delete
            {
                Id = id.Value
            };

            return Handle(command);
        }

        public Guid GetId(LanguageId id) => id?.Value ?? Guid.Empty;

        private async Task HandleCreate(Create cmd)
        {
            if (await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var language = Language.Create(cmd.Id, cmd.Name);

            await Repository.AddAsync(language);

            if (language.EnsureValidState())
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

            var updatableLanguage = await Repository.GetAsync(cmd.Id);

            updatableLanguage.SetName(cmd.Name);
            
            Repository.Update(updatableLanguage);

            if (updatableLanguage.EnsureValidState())
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
                throw new ArgumentNullException();
            }
        }
    }
}
