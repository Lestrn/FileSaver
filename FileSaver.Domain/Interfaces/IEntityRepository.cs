namespace FileSaver.Domain.Interfaces
{
    public interface IEntityRepository<TEntity>
    {
        public Task<List<TEntity>> GetAllAsync();

        public bool Add(TEntity entity);

        public bool Delete(TEntity entity);

        public Task UpdateAsync(TEntity entity);

        public Task SaveChangesAsync();

        public Task<TEntity?> FindByIdAsync(Guid id);

        public Task<TEntity?> FindByIdWithIncludesAsync(Guid id, params string[] includeNames);

        public Task<IEnumerable<TEntity>> Where(Func<TEntity, bool> predicate);

        public Task<IQueryable<TResult>> Select<TResult>(System.Linq.Expressions.Expression<Func<TEntity, TResult>> selector);

        public Task<List<TEntity>> GetAllAsyncWithIncludesAsync(params string[] includeNames);

        public Task<bool> Any(Func<TEntity, bool> predicate);
    }
}
