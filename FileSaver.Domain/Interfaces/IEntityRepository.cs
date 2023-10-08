using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Domain.Interfaces
{
    public interface IEntityRepository<TEntity>
    {
        public Task<List<TEntity>> GetAllAsync();
        public Task AddAsync(TEntity entity);
        public Task DeleteAsync(TEntity entity);
        public Task UpdateAsync(TEntity entity);
        public Task SaveChangesAsync();
        public Task<TEntity?> FindByIdAsync(Guid id);
        public Task<TEntity?> FindByIdWithIncludesAsync(Guid id, params string[] includeNames);
        public Task<IQueryable<TEntity>> WhereQueryable(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate);
        public Task<IEnumerable<TEntity>> WhereEnumerable(Func<TEntity, bool> predicate);
        public Task<IQueryable<TResult>> Select<TResult>(System.Linq.Expressions.Expression<Func<TEntity, TResult>> selector);
        public Task<List<TEntity>> GetAllAsyncWithIncludes(params string[] includeNames);
        public Task<bool> Any(Func<TEntity, bool> predicate);
    }
}
