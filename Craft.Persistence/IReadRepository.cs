using System.Linq.Expressions;

namespace Craft.Persistence;

/// <summary>
/// Queries entities without exposing a persistence provider.
/// </summary>
/// <typeparam name="TEntity">
/// The entity type to query.
/// </typeparam>
/// <remarks>
/// Predicate translation and tracking depend on the implementation. Use explicit updates for portable mutation behavior.
/// </remarks>
public interface IReadRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Counts persisted entities satisfying every predicate (logical AND).
    /// </summary>
    /// <param name="predicates">
    /// Non-null predicates; an empty sequence selects all entities. Expressions must be supported by the provider.
    /// </param>
    /// <param name="cancellationToken">
    /// Requests cancellation of the query.
    /// </param>
    /// <returns>
    /// The number of matching entities.
    /// </returns>
    Task<int> CountAsync(
        IEnumerable<Expression<Func<TEntity, bool>>> predicates,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads persisted entities satisfying every predicate (logical AND).
    /// </summary>
    /// <param name="predicates">
    /// Non-null predicates; an empty sequence selects all entities. Expressions must be supported by the provider.
    /// </param>
    /// <param name="cancellationToken">
    /// Requests cancellation of the query.
    /// </param>
    /// <returns>
    /// Matching entities, or an empty list. No ordering is guaranteed.
    /// </returns>
    /// <remarks>
    /// Navigation loading and change tracking are provider-specific; a read-only list does not make its entities immutable.
    /// </remarks>
    Task<IReadOnlyList<TEntity>> FindAsync(
        IEnumerable<Expression<Func<TEntity, bool>>> predicates,
        CancellationToken cancellationToken = default);
}
