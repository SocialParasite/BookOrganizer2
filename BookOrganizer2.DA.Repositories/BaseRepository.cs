using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.DA;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BookOrganizer2.DA.Repositories
{
    public class BaseRepository<TEntity, TContext, TId> : IRepository<TEntity, TId>
        where TEntity : class
        where TContext : BookOrganizer2DbContext
    {
        protected readonly TContext Context;

        protected BaseRepository(TContext context)
            => Context = context ?? throw new ArgumentNullException(nameof(context));

        //public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        //    => await Context.Set<TEntity>().ToListAsync();

        public virtual async Task<TEntity> GetAsync(TId id)
            => await Context.Set<TEntity>().FindAsync(id);

        public async Task SaveAsync()
            => await Context.SaveChangesAsync();

        public async Task<bool> ExistsAsync(TId id) 
            => await Context.Set<TEntity>().FindAsync(id) != null;

        public async Task AddAsync(TEntity entity) 
            => await Context.Set<TEntity>().AddAsync(entity);

        public void Update(TEntity entity)
            => Context.Update(entity);

        public virtual async Task RemoveAsync(TId id)
        {
            var entity = await Context.Set<TEntity>().FindAsync(id);

            if (entity != null)
                Context.Set<TEntity>().Remove(entity);
        }

        public bool HasChanges()
            => Context.ChangeTracker.HasChanges();

        public void ResetTracking(TEntity entity)
            => Context.Entry(entity).State = EntityState.Unchanged;
    }
}
