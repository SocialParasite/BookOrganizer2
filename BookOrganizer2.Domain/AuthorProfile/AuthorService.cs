using System;
using System.Threading.Tasks;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using static BookOrganizer2.Domain.AuthorProfile.Commands;

namespace BookOrganizer2.Domain.AuthorProfile
{
    public class AuthorService : IAuthorDomainService
    {
        public readonly INationalityLookupDataService NationalityLookupDataService;
        public IRepository<Author, AuthorId> Repository { get; }

        public AuthorService(IRepository<Author, AuthorId> repository,
            INationalityLookupDataService nationalityLookupDataService = null)
        {
            NationalityLookupDataService = nationalityLookupDataService;
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Author CreateItem() 
            => Author.NewAuthor;

        public Task Handle(object command)
        {
            return command switch
            {
                Create cmd => HandleCreate(cmd),
                Update cmd => HandleFullUpdate(cmd),
                SetAuthorsFirstName cmd => HandleUpdate(cmd.Id, (a) => a.SetFirstName(cmd.FirstName), 
                    (a) => Repository.Update(a)),
                SetAuthorsLastName cmd => HandleUpdate(cmd.Id, (a) => a.SetLastName(cmd.LastName), 
                    (a) => Repository.Update(a)),
                SetAuthorDateOfBirth cmd => HandleUpdate(cmd.Id, (a) => a.SetDateOfBirth(cmd.DataOfBirth), 
                    (a) => Repository.Update(a)),
                SetMugshotPath cmd => HandleUpdate(cmd.Id, (a) => a.SetMugshotPath(cmd.MugshotPath), 
                    (a) => Repository.Update(a)),
                SetBiography cmd => HandleUpdate(cmd.Id, (a) => a.SetBiography(cmd.Biography), 
                    (a) => Repository.Update(a)),
                SetNotesOld cmd => HandleUpdate(cmd.Id, (a) => a.SetNotesOld(cmd.NotesOld),
                    (a) => Repository.Update(a)),
                SetNationality cmd => HandleUpdateAsync(cmd.Id, 
                        async a => await UpdateNationalityAsync(a, cmd.NationalityId)),
                DeleteAuthor cmd => HandleUpdate(cmd.Id, _ => Repository.RemoveAsync(cmd.Id)),
                _ => Task.CompletedTask
            };
        }

        public Guid GetId(AuthorId id) => id?.Value ?? Guid.Empty;
        public Task Update(Author model)
        {
            var command = new Update
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                MugshotPath = model.MugshotPath,
                Biography = model.Biography,
                NotesOld = model.NotesOld,
                Nationality = model.Nationality,
                Notes = model.Notes
            };

            return Handle(command);
        }

        public Task RemoveAsync(AuthorId id)
        {
            var command = new DeleteAuthor
            {
                Id = id
            };

            return Handle(command);
        }

        public async Task<Author> AddNew(Author model)
        {
            var command = new Create
            {
                Id = new AuthorId(SequentialGuid.NewSequentialGuid()),
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                MugshotPath = model.MugshotPath,
                Biography = model.Biography,
                NotesOld = model.NotesOld,
                Nationality = model.Nationality,
                Notes = model.Notes
            };

            await Handle(command);

            return await Repository.GetAsync(command.Id);
        }

        private async Task HandleCreate(Create cmd)
        {
            if (await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var author = Author.Create(cmd.Id, 
                                       cmd.FirstName, 
                                       cmd.LastName,
                                       cmd.DateOfBirth,
                                       cmd.Biography,
                                       cmd.MugshotPath,
                                       cmd.NotesOld,
                                       cmd.Notes);

            await Repository.AddAsync(author);

            if (cmd.Nationality is not null)
            {
                await UpdateNationalityAsync(author, cmd.Nationality.Id);
            }

            if (author.EnsureValidState())
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

            var updatableAuthor = await Repository.GetAsync(cmd.Id);

            updatableAuthor.SetFirstName(cmd.FirstName);
            updatableAuthor.SetLastName(cmd.LastName);
            updatableAuthor.SetDateOfBirth(cmd.DateOfBirth);
            updatableAuthor.SetBiography(cmd.Biography);
            updatableAuthor.SetMugshotPath(cmd.MugshotPath);
            updatableAuthor.SetNotesOld(cmd.NotesOld);
            updatableAuthor.SetNationality(cmd.Nationality);
            updatableAuthor.SetNotes(cmd.Notes);

            Repository.Update(updatableAuthor);

            if (updatableAuthor.EnsureValidState())
            {
                await Repository.SaveAsync();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        private async Task HandleUpdate(Guid id, Action<Author> operation, Action <Author> operation2 = null)
        {
            if (await Repository.ExistsAsync(id))
            {
                var author = await Repository.GetAsync(id);
                operation(author);
                operation2?.Invoke(author);

                if (author.EnsureValidState())
                {
                    await Repository.SaveAsync();
                }
            }
            else
                throw new ArgumentException();
        }

        private async Task HandleUpdateAsync(Guid authorId, Func<Author, Task> operation)
        {
            if (await Repository.ExistsAsync(authorId))
            {
                var author = await Repository.GetAsync(authorId);

                if (author is null)
                    throw new InvalidOperationException($"Entity with id {authorId} cannot be found");

                await operation(author);

                if (author.EnsureValidState())
                {
                    await Repository.SaveAsync();
                }
            }
            else
                throw new ArgumentException();
        }

        private Task UpdateNationalityAsync(Author author, NationalityId nationalityId) 
            => ((IAuthorRepository) Repository).ChangeNationality(author, nationalityId);
    }
}
