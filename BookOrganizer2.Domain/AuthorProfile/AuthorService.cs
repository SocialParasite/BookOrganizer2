using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using System;
using System.Threading.Tasks;
using static BookOrganizer2.Domain.AuthorProfile.Commands;

namespace BookOrganizer2.Domain.AuthorProfile
{
    public class AuthorService : IDomainService<Author, AuthorId>
    {
        public IRepository<Author, AuthorId> Repository { get; }
        public Author CreateItem()
        {
            return Author.NewAuthor;
        }

        public AuthorService(IRepository<Author, AuthorId> repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task Handle(object command)
        {
            return command switch
            {
                Create cmd => HandleCreate(cmd),
                Update cmd => HandleUpdate(cmd),
                SetAuthorsFirstName cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetFirstName(cmd.FirstName), (a) => Repository.Update(a)),
                SetAuthorsLastName cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetLastName(cmd.LastName), (a) => Repository.Update(a)),
                SetAuthorDateOfBirth cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetDateOfBirth(cmd.DataOfBirth), (a) => Repository.Update(a)),
                SetMugshotPath cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetMugshotPath(cmd.MugshotPath), (a) => Repository.Update(a)),
                SetBiography cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetBiography(cmd.Biography), (a) => Repository.Update(a)),
                SetNotes cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetNotes(cmd.Notes), (a) => Repository.Update(a)),
                DeleteAuthor cmd => HandleUpdateAsync(cmd.Id, _ => Repository.RemoveAsync(cmd.Id)),
                _ => Task.CompletedTask
            };
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
                                       cmd.Notes);

            await Repository.AddAsync(author);

            if (author.EnsureValidState())
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

            var updatableAuthor = await Repository.GetAsync(cmd.Id);

            updatableAuthor.SetFirstName(cmd.FirstName);
            updatableAuthor.SetLastName(cmd.LastName);
            updatableAuthor.SetDateOfBirth(cmd.DateOfBirth);
            updatableAuthor.SetBiography(cmd.Biography);
            updatableAuthor.SetMugshotPath(cmd.MugshotPath);
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

        private async Task HandleUpdateAsync(Guid id, Action<Author> operation, Action <Author> operation2 = null)
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
    }
}
