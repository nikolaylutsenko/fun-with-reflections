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
    public async Task<OneOf<TEntity?, IEnumerable<TEntity>>> GetAsync(
                Expression<Func<TEntity, bool>>? filter = null,
                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                Amount amount = Amount.One,
                params string[] includes)
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

        switch (amount)
        {
            case Amount.One:
                var one = await query.FirstOrDefaultAsync();
                return one;
            case Amount.All:
                var all = await query.ToListAsync();
                return all;
            default:
                throw new Exception("Out of bounces");
        }
    }
}

