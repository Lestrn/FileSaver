using FileSaver.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Infrastructure.Persistence
{
    public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly DbSet<TEntity> _dbSet;

        private readonly FileSaverContext _context;

        public EntityRepository(FileSaverContext dbContext)
        {
            _context = dbContext;
            _dbSet = _context.Set<TEntity>();
        }
        public bool Add(TEntity entity)
        {
            if(entity == null)
            {
                return false;
            }
            _context.Add(entity);
            return true;
           
        }

        public bool DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                return false;
            }
            _context.Remove(entity);
            return true;
        }

        public Task<TEntity?> FindByIdAsync(Guid id)
        {
            return _dbSet.FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            return _dbSet.ToListAsync();
        }
        public Task<List<TEntity>> GetAllAsyncWithIncludesAsync(params string[] includeNames)
        {
            IQueryable<TEntity> query = _dbSet;
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
            return Task.FromResult(_dbSet.Where(predicate));  
        }
        public Task<IQueryable<TResult>> Select<TResult>(System.Linq.Expressions.Expression<Func<TEntity, TResult>> selector)
        {
            return Task.FromResult(_dbSet.Select(selector));
        }
        public Task<bool> Any(Func<TEntity, bool> predicate)
        {
            return Task.FromResult(_dbSet.Any(predicate));
        }
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
        public Task<TEntity?> FindByIdWithIncludesAsync(Guid id, params string[] includeNames)
        {
            if (includeNames == null)
            {
                throw new ArgumentNullException("Include names can't be null");
            }

            IQueryable<TEntity> query = _dbSet;
            try
            {
                foreach (var includeName in includeNames)
                {
                    query = query.Include(includeName);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return query.FirstOrDefaultAsync(entity => entity.Id == id);
        }
    }
}
