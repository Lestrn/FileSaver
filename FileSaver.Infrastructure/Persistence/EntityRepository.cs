namespace FileSaver.Infrastructure.Persistence
{
    using FileSaver.Domain.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class EntityRepository<TEntity> : IEntityRepository<TEntity>
        where TEntity : class, IEntity
    {
        private readonly DbSet<TEntity> dbSet;

        private readonly FileSaverContext context;

        public EntityRepository(FileSaverContext dbContext)
        {
            this.context = dbContext;
            this.dbSet = this.context.Set<TEntity>();
        }

        public bool Add(TEntity entity)
        {
            if (entity == null)
            {
                return false;
            }

            this.context.Add(entity);

            return true;
        }

        public bool Delete(TEntity entity)
        {
            if (entity == null)
            {
                return false;
            }

            this.context.Remove(entity);

            return true;
        }

        public Task<TEntity?> FindByIdAsync(Guid id)
        {
            return this.dbSet.FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            return this.dbSet.ToListAsync();
        }

        public Task<List<TEntity>> GetAllAsyncWithIncludesAsync(params string[] includeNames)
        {
            IQueryable<TEntity> query = this.dbSet;
            try
            {
                foreach (var includeName in includeNames)
                {
                    query = query.Include(includeName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return query.ToListAsync();
        }

        public Task<IEnumerable<TEntity>> Where(Func<TEntity, bool> predicate)
        {
            return Task.FromResult(this.dbSet.Where(predicate));
        }

        public Task<IQueryable<TResult>> Select<TResult>(System.Linq.Expressions.Expression<Func<TEntity, TResult>> selector)
        {
            return Task.FromResult(this.dbSet.Select(selector));
        }

        public Task<bool> Any(Func<TEntity, bool> predicate)
        {
            return Task.FromResult(this.dbSet.Any(predicate));
        }

        public Task SaveChangesAsync()
        {
            return this.context.SaveChangesAsync();
        }

        public Task UpdateAsync(TEntity entity)
        {
            this.dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public Task<TEntity?> FindByIdWithIncludesAsync(Guid id, params string[] includeNames)
        {
            if (includeNames == null)
            {
                throw new ArgumentNullException("Include names can't be null");
            }

            IQueryable<TEntity> query = this.dbSet;
            try
            {
                foreach (var includeName in includeNames)
                {
                    query = query.Include(includeName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return query.FirstOrDefaultAsync(entity => entity.Id == id);
        }
    }
}
