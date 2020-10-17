using System;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using static BookOrganizer2.Domain.AuthorProfile.Commands;

namespace BookOrganizer2.Domain.AuthorProfile
{
    public class AuthorService : IDomainService<Author, AuthorId>
    {
        private readonly IRepository<Author, AuthorId> _repository;

        public AuthorService(IRepository<Author, AuthorId> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task Handle(object command)
        {
            return command switch
            {
                Create cmd => HandleCreate(cmd),
                SetAuthorsFirstName cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetFirstName(cmd.FirstName), (a) => _repository.Update(a)),
                SetAuthorsLastName cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetLastName(cmd.LastName), (a) => _repository.Update(a)),
                SetAuthorDateOfBirth cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetDateOfBirth(cmd.DataOfBirth), (a) => _repository.Update(a)),
                SetMugshotPath cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetMugshotPath(cmd.MugshotPath), (a) => _repository.Update(a)),
                SetBiography cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetBiography(cmd.Biography), (a) => _repository.Update(a)),
                SetNotes cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetNotes(cmd.Notes), (a) => _repository.Update(a)),
                DeleteAuthor cmd => HandleUpdateAsync(cmd.Id, _ => _repository.RemoveAsync(cmd.Id)),
                _ => Task.CompletedTask
            };
        }

        private async Task HandleCreate(Create cmd)
        {
            if (await _repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var author = Author.Create(cmd.Id, 
                                       cmd.FirstName, 
                                       cmd.LastName,
                                       cmd.DateOfBirth,
                                       cmd.Biography,
                                       cmd.MugshotPath,
                                       cmd.Notes);

            await _repository.AddAsync(author);

            if (author.EnsureValidState())
            {
                await _repository.SaveAsync();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        private async Task HandleUpdateAsync(Guid id, Action<Author> operation, Action <Author> operation2 = null)
        {
            if (await _repository.ExistsAsync(id))
            {
                var author = await _repository.GetAsync(id);
                operation(author);
                operation2?.Invoke(author);

                if (author.EnsureValidState())
                {
                    await _repository.SaveAsync();
                }
            }
            else
                throw new ArgumentException();
        }
    }
}
