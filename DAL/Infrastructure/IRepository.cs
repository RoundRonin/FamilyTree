using System.Linq.Expressions;

namespace DAL.Infrastructure;

internal interface IRepository<T> where T : class, IEntity
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T?> RetrieveByIdAsync(int id); 
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    
}
