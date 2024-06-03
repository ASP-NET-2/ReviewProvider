using Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public abstract class Repository<TEntity, TDbContext>(/*TDbContext dataContext*/) where TEntity : class where TDbContext : DbContext
{
    //private readonly TDbContext _dataContext = dataContext;
    
    //public TDbContext Context => _dataContext;

    //public async Task<int> SaveChangesAsync() => await Context.SaveChangesAsync();

    public virtual IQueryable<TEntity> GetSet(TDbContext context, bool includeRelations)
    {
        return context.Set<TEntity>();
    }

    public virtual async Task<TEntity> CreateAsync(TDbContext context, TEntity entity, bool saveChanges = true)
    {
        try
        {
            var result = await context.Set<TEntity>().AddAsync(entity);
            if (saveChanges)
                await context.SaveChangesAsync();
            Debug.WriteLine(context.ChangeTracker.Entries().Count());
            return result.Entity;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return null!;
    }

    public virtual async Task<TEntity?> GetAsync(TDbContext context, Expression<Func<TEntity, bool>> expression, bool includeRelations = false)
    {
        try
        {
            var entity = await GetSet(context, includeRelations).FirstOrDefaultAsync(expression);
            return entity ?? null;
        }
        catch(Exception e) { Debug.WriteLine(e.Message); }

        return null;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(TDbContext context, bool includeRelations = false)
    {
        var entities = await GetSet(context, includeRelations).ToListAsync();
        return entities ?? null!;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(TDbContext context, Expression<Func<TEntity, bool>> expression, bool includeRelations = false)
    {
        var entities = await GetSet(context, includeRelations).Where(expression).ToListAsync();
        return entities ?? null!;
    }

    public virtual async Task<bool> ExistsAsync(TDbContext context, Expression<System.Func<TEntity, bool>> expression)
    {
        return await context.Set<TEntity>().AnyAsync(expression);
    }

    public virtual async Task<TEntity> UpdateAsync(TDbContext context, TEntity entity, bool saveChanges = true)
    {
        var result = context.Set<TEntity>().Update(entity);
        if (saveChanges)
            await context.SaveChangesAsync();
        Debug.WriteLine(context.ChangeTracker.Entries().Count());
        return result.Entity;
    }

    public virtual async Task<bool> DeleteAsync(TDbContext context, TEntity entity, bool saveChanges = true)
    {
        try
        {
            context.Set<TEntity>().Remove(entity);
            if (saveChanges)
                await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return false;
    }
}
