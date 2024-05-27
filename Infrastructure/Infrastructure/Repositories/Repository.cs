using Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public abstract class Repository<TEntity, TDbContext>(TDbContext dataContext) where TEntity : class where TDbContext : DbContext
{
    private readonly TDbContext _dataContext = dataContext;
    
    protected TDbContext Context => _dataContext;

    public virtual IQueryable<TEntity> GetSet(bool includeRelations)
    {
        return Context.Set<TEntity>();
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        try
        {
            var result = await Context.Set<TEntity>().AddAsync(entity);
            await Context.SaveChangesAsync();
            return result.Entity;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return null!;
    }

    public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression, bool includeRelations = false)
    {
        try
        {
            var entity = await GetSet(includeRelations).FirstOrDefaultAsync(expression);
            return entity ?? null;
        }
        catch(Exception e) { Debug.WriteLine(e.Message); }

        return null;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool includeRelations = false)
    {
        var entities = await GetSet(includeRelations).ToListAsync();
        return entities ?? null!;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, bool includeRelations = false)
    {
        var entities = await GetSet(includeRelations).Where(expression).ToListAsync();
        return entities ?? null!;
    }

    public virtual async Task<bool> ExistsAsync(Expression<System.Func<TEntity, bool>> expression)
    {
        return await Context.Set<TEntity>().AnyAsync(expression);
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var result = Context.Set<TEntity>().Update(entity);
        await Context.SaveChangesAsync();
        return result.Entity;
    }

    public virtual async Task<bool> DeleteAsync(TEntity entity)
    {
        try
        {
            Context.Set<TEntity>().Remove(entity);
            await Context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return false;
    }
}
