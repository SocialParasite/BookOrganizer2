using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.DA;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookOrganizer2.DA.Repositories
{
    public class BaseRepository<TEntity, TContext> : IRepository<TEntity>
        where TEntity : class
        where TContext : BookOrganizer2DbContext
    {
        protected readonly TContext Context;

        protected BaseRepository(TContext context) 
            => Context = context ?? throw new ArgumentNullException(nameof(context));

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
            => await Context.Set<TEntity>().ToListAsync();

        public virtual async Task<TEntity> GetSelectedAsync(Guid id)
            => await Context.Set<TEntity>().FindAsync(id);

        public async Task SaveAsync()
            => await Context.SaveChangesAsync();

        public void Update(TEntity entity)
            => Context.Update(entity);

        public void Delete(TEntity entity)
            => Context.Remove(entity);

        public bool HasChanges()
            => Context.ChangeTracker.HasChanges();

        public void ResetTracking(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Unchanged;
        }
    }
}
