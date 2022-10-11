using System.Linq;

namespace MusicLibrarySuite.CatalogService.Data.Repositories;

/// <summary>
/// Encapsulates a method that performs processing of an existring <see cref="IQueryable{T}" /> object containing entities of <typeparamref name="TEntity" /> type.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <param name="collection">The existing <see cref="IQueryable{T}" /> object.</param>
/// <returns>An <see cref="IQueryable{T}" /> object representing the results of performed filtering.</returns>
public delegate IQueryable<TEntity> EntityCollectionProcessor<TEntity>(IQueryable<TEntity> collection) where TEntity : class;
