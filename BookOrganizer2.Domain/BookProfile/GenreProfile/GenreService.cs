using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using System;
using System.Threading.Tasks;
using static BookOrganizer2.Domain.BookProfile.GenreProfile.Commands;

namespace BookOrganizer2.Domain.BookProfile.GenreProfile
{
    public class GenreService : IGenreService
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
                Delete cmd => HandleDeleteAsync(cmd),
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
        public async Task<Genre> AddNew(string name)
        {
            var command = new Create
            {
                Id = new GenreId(SequentialGuid.NewSequentialGuid()),
                Name = name
            };

            await Handle(command);

            return await Repository.GetAsync(command.Id);
        }

        public Guid GetId(GenreId id) => id?.Value ?? Guid.Empty;
        public Task Update(Genre model)
        {
            var command = new Update
            {
                Id = model.Id,
                Name = model.Name
            };

            return Handle(command);
        }

        public Task RemoveAsync(GenreId id)
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
