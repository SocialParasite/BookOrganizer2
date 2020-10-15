using BookOrganizer2.Domain.DA;
using System;

namespace BookOrganizer2.Domain.Services
{
    public class AuthorService : IDomainService<Author>
    {
        public IRepository<Author> Repository { get; }

        public AuthorService(IRepository<Author> repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public Author CreateItem()
        {
            return new Author();
        }
    }
}
