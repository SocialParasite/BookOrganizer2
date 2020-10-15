using BookOrganizer2.Domain.DA;
using System;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.Services
{
    public class AuthorService : IDomainService<Author, AuthorId>
    {
        private readonly IRepository<Author, AuthorId> _repository;

        public AuthorService(IRepository<Author, AuthorId> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task HandleCreate(Author author)
        {
            if (await _repository.ExistsAsync(author.Id))
                throw new InvalidOperationException($"Entity with id {author.Id} already exists");

            await _repository.AddAsync(author);
            await _repository.SaveAsync();
        }
    }
}
