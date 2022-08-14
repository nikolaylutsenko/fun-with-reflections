using System.Linq.Expressions;
using FunWithReflection.Console.Entities;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace FunWithReflection.Console;

public class Repository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet;

    public Repository(DbContext context)
    {
        _dbSet = context.Set<TEntity>();
    }

    // this method receive as TResult only POCO class or class implemented generic IEnumerable<>
    public async Task<OneOf<TEntity?, IEnumerable<TEntity>>> GetAsync<TResult>(
                Expression<Func<TEntity, bool>>? filter = null,
                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                params string[] includes) where TResult : class
    {
        var query = _dbSet as IQueryable<TEntity>;
        var l = await query.ToListAsync();

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
        if (typeof(TResult).GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>))) // if inner type is null it means that we provide POCO type    
        {
            var result = await query.FirstOrDefaultAsync();
            return result;
        }
        // else we provide generic type we must check if it derives from IEnumerable<>    
        else if (typeof(TResult).GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            var result = await query.ToListAsync();
            return result;
        }
        else // if none of those, we don't deal with it    
        {
            throw new Exception($"Not supported type {typeof(TResult).Name}");
        }
    }
}

