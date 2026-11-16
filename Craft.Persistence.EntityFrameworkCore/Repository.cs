//using System.Linq.Expressions;
//using Microsoft.EntityFrameworkCore;

//namespace Craft.Persistence.EntityFrameworkCore
//{
//    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
//    {
//        protected readonly DbContext Context;

//        protected Repository(
//            DbContext context)
//        {
//            Context = context;
//        }

//        public int CountAll()
//        {
//            return Context.Set<TEntity>().Count();
//        }

//        public int Count(
//            Expression<Func<TEntity, bool>> predicate)
//        {
//            return Context.Set<TEntity>().Count(predicate);
//        }

//        public int Count(
//            IList<Expression<Func<TEntity, bool>>> predicates)
//        {
//            var predicate = predicates.Aggregate((c, n) => c.And(n));
//            return Context.Set<TEntity>().Count(predicate);
//        }

//        public async Task<IEnumerable<TEntity>> GetAll()
//        {
//            return await Task.Run(() =>
//            {
//                return Context.Set<TEntity>().ToList();
//            });
//        }

//        public async Task<IEnumerable<TEntity>> Find(
//            Expression<Func<TEntity, bool>> predicate)
//        {
//            return await Task.Run(() =>
//            {
//                return Context.Set<TEntity>().Where(predicate);
//            });
//        }

//        public async Task<IEnumerable<TEntity>> Find(
//            IList<Expression<Func<TEntity, bool>>> predicates)
//        {
//            return await Task.Run(async () =>
//            {
//                if (!predicates.Any())
//                {
//                    return await GetAll();
//                }

//                var predicate = predicates.Aggregate((c, n) => c.And(n));
//                return Context.Set<TEntity>().Where(predicate);
//            });
//        }

//        public TEntity SingleOrDefault(
//            Expression<Func<TEntity, bool>> predicate)
//        {
//            return Context.Set<TEntity>().SingleOrDefault(predicate);
//        }

//        public virtual async Task Add(
//            TEntity entity)
//        {
//            await Task.Run(() =>
//            {
//                Context.Set<TEntity>().Add(entity);
//            });
//        }

//        public async Task AddRange(
//            IEnumerable<TEntity> entities)
//        {
//            await Task.Run(() =>
//            {
//                Context.Set<TEntity>().AddRange(entities);
//            });
//        }

//        public async Task Remove(
//            TEntity entity)
//        {
//            Context.Set<TEntity>().Remove(entity);
//        }

//        public abstract Task Clear();

//        public abstract Task Update(
//            TEntity entity);

//        public abstract Task UpdateRange(
//            IEnumerable<TEntity> entities);

//        public async Task RemoveRange(
//            IEnumerable<TEntity> entities)
//        {
//            await Task.Run(() =>
//            {
//                Context.Set<TEntity>().RemoveRange(entities);
//            });
//        }

//        public void Load(
//            IEnumerable<TEntity> entities)
//        {
//            Context.Set<TEntity>().AddRange(entities);
//            Context.SaveChanges();

//            /*
//            // Notice that we use Z.EntityFramework.Extensions for this ..... 
//            // (Dette gjorde jeg for lang tid siden for at addressere problemet med at man ikke uden videre kan sætte værdien
//            // for identity attributes. Det er imidlertid ikke uden implikationer, f.eks. derved at Z.EntityFramework er et
//            // kommercielt tool, der tilsyneladende kun virker i en begrænset periode efter at man har bygget sin applikation

//            var skip = 0;
//            var bufferCapacity = 10;
//            var bufferSize = 0;

//            do
//            {
//                var buffer = entities
//                    .Skip(skip)
//                    .Take(bufferCapacity)
//                    .ToList();

//                bufferSize = buffer.Count();

//                if (bufferSize > 0)
//                {
//                    Context.BulkInsert(
//                        buffer,
//                        options => options.InsertKeepIdentity = true);
//                }

//                skip += bufferCapacity;

//                Console.Write(".");
//            } while (bufferSize > 0);

//            // Original
//            //Context.BulkInsert(entities, options => options.InsertKeepIdentity = true);
//            */
//        }
//    }
//}

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Craft.Persistence.EFCore;

/// <summary>
/// Uses an EF Core context to query entities and stage changes.
/// </summary>
/// <typeparam name="TEntity">
/// An entity type mapped by the supplied context.
/// </typeparam>
/// <remarks>
/// Does not own or dispose the context. Queries use its configured tracking policy (tracking by default), apply predicates with AND, and do not explicitly include navigations. Mutations use EF graph/state rules; Update may mark all mapped values modified and attach reachable entities. No method saves changes.
/// </remarks>
public abstract class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// The context shared with the owning persistence session.
    /// </summary>
    protected readonly DbContext Context;

    /// <summary>
    /// Creates a repository using an externally owned context.
    /// </summary>
    /// <param name="context">
    /// The context used for all operations.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The context is null.
    /// </exception>
    protected Repository(
        DbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(
        IEnumerable<Expression<Func<TEntity, bool>>> predicates,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyPredicates(predicates);

        return await query.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> FindAsync(
        IEnumerable<Expression<Func<TEntity, bool>>> predicates,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyPredicates(predicates);

        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Context.Set<TEntity>().Add(entity);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Context.Set<TEntity>().Update(entity);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Context.Set<TEntity>().Remove(entity);

        return Task.CompletedTask;
    }

    private IQueryable<TEntity> ApplyPredicates(
        IEnumerable<Expression<Func<TEntity, bool>>> predicates)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();

        foreach (var predicate in predicates)
        {
            query = query.Where(predicate);
        }

        return query;
    }
}
