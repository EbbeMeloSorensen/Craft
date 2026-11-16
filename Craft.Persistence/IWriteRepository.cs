namespace Craft.Persistence;

/// <summary>
/// Stages entity mutations for a separate commit by the owning persistence session.
/// </summary>
/// <typeparam name="TEntity">
/// The entity type to mutate.
/// </typeparam>
/// <remarks>
/// Duplicate, missing-entity and entity-graph behavior depend on the provider. Completion of these methods does not mean changes have been persisted.
/// </remarks>
public interface IWriteRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Stages insertion of an entity; does not commit changes.
    /// </summary>
    /// <param name="entity">
    /// The non-null entity whose mutation is staged.
    /// </param>
    /// <param name="cancellationToken">
    /// Requests cancellation of staging.
    /// </param>
    /// <returns>
    /// A task representing staging, which may complete synchronously.
    /// </returns>
    Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages an explicit update of an entity; does not commit changes.
    /// </summary>
    /// <param name="entity">
    /// The non-null entity whose mutation is staged.
    /// </param>
    /// <param name="cancellationToken">
    /// Requests cancellation of staging.
    /// </param>
    /// <returns>
    /// A task representing staging, which may complete synchronously.
    /// </returns>
    Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages deletion of an entity; does not commit changes.
    /// </summary>
    /// <param name="entity">
    /// The non-null entity whose mutation is staged.
    /// </param>
    /// <param name="cancellationToken">
    /// Requests cancellation of staging.
    /// </param>
    /// <returns>
    /// A task representing staging, which may complete synchronously.
    /// </returns>
    Task RemoveAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);
}
