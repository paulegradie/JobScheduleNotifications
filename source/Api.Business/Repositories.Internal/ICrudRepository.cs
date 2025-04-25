namespace Api.Business.Repositories.Internal;

// TODO: This should only live in the infra - we should use these in repositories - this is for pulling
// entities out of the db
public interface ICrudRepository<T, in TKey> where T : class
{
    Task<T?> GetByIdAsync(TKey id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveChangesAsync();
    IQueryable<T> Query();
} 