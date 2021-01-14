using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using System;
using System.Threading.Tasks;
using static BookOrganizer2.Domain.BookProfile.GenreProfile.Commands;

namespace BookOrganizer2.Domain.BookProfile.GenreProfile
{
    public class GenreService : ISimpleDomainService<Genre, GenreId>
    {
        public IRepository<Genre, GenreId> Repository { get; }

        public GenreService(IRepository<Genre, GenreId> repository)
            => Repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public Genre CreateItem() => Genre.NewGenre;

        public Task Handle(object command)
        {
            return command switch
            {
                Create cmd => HandleCreate(cmd),
                Update cmd => HandleUpdate(cmd),
                DeleteGenre cmd => HandleUpdateAsync(cmd.Id, _ => Repository.RemoveAsync(cmd.Id)),
                _ => Task.CompletedTask
            };
        }

        public async Task<Genre> AddNew(Genre model)
        {
            var command = new Create
            {
                Id = new GenreId(SequentialGuid.NewSequentialGuid()),
                Name = model.Name
            };

            await Handle(command);

            return await Repository.GetAsync(command.Id);
        }

        public Guid GetId(GenreId id) => id?.Value ?? Guid.Empty;

        private async Task HandleCreate(Create cmd)
        {
            if (await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var genre = Genre.Create(cmd.Id, cmd.Name);

            await Repository.AddAsync(genre);

            if (genre.EnsureValidState())
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

            var updatableGenre = await Repository.GetAsync(cmd.Id);

            updatableGenre.SetName(cmd.Name);
            
            Repository.Update(updatableGenre);

            if (updatableGenre.EnsureValidState())
            {
                await Repository.SaveAsync();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        private async Task HandleUpdateAsync(Guid id, Action<Genre> operation, Action<Genre> operation2 = null)
        {
            if (await Repository.ExistsAsync(id))
            {
                var genre = await Repository.GetAsync(id);
                operation(genre);
                operation2?.Invoke(genre);

                if (genre.EnsureValidState())
                {
                    await Repository.SaveAsync();
                }
            }
            else
                throw new ArgumentException();
        }
    }
}
