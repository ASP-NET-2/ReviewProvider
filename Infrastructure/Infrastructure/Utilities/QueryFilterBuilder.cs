//namespace Infrastructure.Utilities;

//public abstract class QueryFilterBuilder<TEntity> where TEntity : class
//{
//    protected IReadOnlyDictionary<string, QueryFilter<TEntity>> _queryFilters = new Dictionary<string, QueryFilter<TEntity>>();

//    public IQueryable<TEntity> FilterQuery(IQueryable<TEntity> dbSet, QueryFilterIdentifier[]? filterIdentifiers)
//    {
//        if (filterIdentifiers == null || filterIdentifiers.Length == 0)
//            return dbSet;

//        foreach (var identifier in filterIdentifiers)
//        {

//        }
//    }
//}

//public class QueryFilterIdentifier
//{
//    public string Identifier { get; set; } = null!;
//    public object? Value { get; set; }
//}

//public abstract class QueryFilter<TEntity> where TEntity : class
//{
//    public abstract IQueryable<TEntity> FilterQuery(IQueryable<TEntity> contextQuery);
//}