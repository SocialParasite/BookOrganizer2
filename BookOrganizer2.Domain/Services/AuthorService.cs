using BookOrganizer2.Domain.DA;
using System;
using System.Threading.Tasks;
using static BookOrganizer2.Domain.Commands;
namespace BookOrganizer2.Domain.Services
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
                SetAuthorsFirstName cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetFirstName(cmd.FirstName)),
                SetAuthorsLastName cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetLastName(cmd.LastName)),
                SetAuthorDateOfBirth cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetDateOfBirth(cmd.DataOfBirth)),
                SetMugshotPath cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetMugshotPath(cmd.MugshotPath)),
                SetBiography cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetBiography(cmd.Biography)),
                SetNotes cmd => HandleUpdateAsync(cmd.Id, (a) => a.SetNotes(cmd.Notes)),
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

        private async Task HandleUpdateAsync(Guid id, Action<Author> operation)
        {
            if (await _repository.ExistsAsync(id))
            {
                var author = await _repository.GetAsync(id);
                operation(author);

                if (author.EnsureValidState())
                {
                    _repository.Update(author);
                    await _repository.SaveAsync();
                }
            }
            else
                throw new ArgumentException();
        }
    }
}
