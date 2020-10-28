using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.DA;
using System;
using System.Threading.Tasks;

namespace BookOrganizer2.DA.Repositories
{
    public class BaseRepository<TEntity, TContext, TId> : IRepository<TEntity, TId>
        where TEntity : class
        where TContext : BookOrganizer2DbContext
    {
        private readonly TContext _context;

        protected BaseRepository(TContext context)
            => _context = context ?? throw new ArgumentNullException(nameof(context));

        //public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        //    => await Context.Set<TEntity>().ToListAsync();

        public async Task<TEntity> GetAsync(TId id)
            => await _context.Set<TEntity>().FindAsync(id);

        public async Task SaveAsync()
            => await _context.SaveChangesAsync();

        public async Task<bool> ExistsAsync(TId id) 
            => await _context.Set<TEntity>().FindAsync(id) != null;

        public async Task AddAsync(TEntity entity) 
            => await _context.Set<TEntity>().AddAsync(entity);

        public void Update(TEntity entity)
            => _context.Update(entity);

        public async Task RemoveAsync(TId id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);

            if (entity != null)
                _context.Set<TEntity>().Remove(entity);
        }

        public bool HasChanges()
            => _context.ChangeTracker.HasChanges();

        //public void ResetTracking(TEntity entity) 
        //    => Context.Entry(entity).State = EntityState.Unchanged;
    }
}
