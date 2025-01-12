using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using FamilyTreeBlazor.DAL.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyTreeBlazor.DAL;
public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private readonly IServiceScopeFactory _scopeFactory;

    public Repository(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task AddAsync(T entity)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var dbSet = context.Set<T>();
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(T entity)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var dbSet = context.Set<T>();
            dbSet.Update(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(T entity)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var dbSet = context.Set<T>();
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var dbSet = context.Set<T>();
            T entity = await dbSet.FindAsync(id)
                ?? throw new InvalidOperationException($"Entity of type {typeof(T)} with Id {id} not found.");
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task<T?> RetrieveByIdAsync(int id)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var dbSet = context.Set<T>();
            return await dbSet.FindAsync(id);
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var dbSet = context.Set<T>();
            return await dbSet.ToListAsync();
        }
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var dbSet = context.Set<T>();
            return await dbSet.Where(predicate).ToListAsync();
        }
    }

    public async Task TruncateTableAsync()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var tableName = context.Model.FindEntityType(typeof(T)).GetTableName();

            // Ensure the table name is quoted to preserve case sensitivity
            tableName = $"\"{tableName}\"";

            try
            {
                await context.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE {tableName}");
            }
            catch (DbUpdateException dbEx) when (IsTableNotFoundException(dbEx))
            {
                throw new InvalidOperationException($"Table '{tableName}' not found.", dbEx);
            }
        }
    }

    private bool IsTableNotFoundException(DbUpdateException dbEx)
    {
        return dbEx.InnerException is PostgresException postgresEx &&
               postgresEx.SqlState == "42P01"; // "Relation does not exist" exception
    }
}
