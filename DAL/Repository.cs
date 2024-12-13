using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using FamilyTreeBlazor.DAL.Infrastructure;

namespace FamilyTreeBlazor.DAL;
public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int Id)
    {
        T entity = await RetrieveByIdAsync(Id) 
            ?? throw new InvalidOperationException($"Entity of type {typeof(T)} with Id {Id} not found.");

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<T?> RetrieveByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task TruncateTableAsync()
    {
        var tableName = _context.Model.FindEntityType(typeof(T)).GetTableName();
        try
        {
            await _context.Database.ExecuteSqlAsync($"TRUNCATE TABLE {tableName}");
        }
        catch (DbUpdateException dbEx) when (IsTableNotFoundException(dbEx))
        {
            throw new InvalidOperationException($"Table '{tableName}' not found.", dbEx);
        }
    }

    private bool IsTableNotFoundException(DbUpdateException dbEx)
    {
        return dbEx.InnerException is PostgresException postgresEx &&
               postgresEx.SqlState == "42P01"; // "Relation does not exist" exception
    }

}
