// this method receive as TResult only POCO class or class implemented generic IEnumerable<>
public async Task<TResult?> GetAsync<TResult>(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params string[] includes) where TResult : class
{
    var query = _dbSet as IQueryable<TEntity>;

    if (filter != null)
    {
        query = query.Where(filter);
    }

    if (includes.Any())
    {
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
    }

    if (orderBy != null)
    {
        query = orderBy(query);
    }

    // add just a bit of black magic
    var innerType = typeof(TResult).GetGenericArguments().FirstOrDefault();
    if (typeof(TResult).GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>))) // if implement IEntity it means that we wont to receive single entity
    {
        var result = (await query.FirstOrDefaultAsync()) as TResult;
        return result;
    } // else we provide generic type we must check if it derives from IEnumerable<>
    else if (typeof(TResult).GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
    {
        var result = (await query.ToListAsync()) as TResult;
        return result;
    }
    else // if none of those, we don't deal with it
    {
        throw new Exception($"Not supported type {typeof(TResult).Name}");
    }
}