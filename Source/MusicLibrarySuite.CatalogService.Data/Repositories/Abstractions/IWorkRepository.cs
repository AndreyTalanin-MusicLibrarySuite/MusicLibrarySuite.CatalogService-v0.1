using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

/// <summary>
/// Defines a set of members a provider-specific work repository should implement.
/// </summary>
public interface IWorkRepository
{
    /// <summary>
    /// Asynchronously gets a work by its unique identifier.
    /// </summary>
    /// <param name="workId">The work's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the work found or <see langword="null" />.
    /// </returns>
    public Task<WorkDto?> GetWorkAsync(Guid workId);

    /// <summary>
    /// Asynchronously gets all works.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all works.
    /// </returns>
    public Task<WorkDto[]> GetWorksAsync();

    /// <summary>
    /// Asynchronously gets works by a collection of unique identifiers.
    /// </summary>
    /// <param name="workIds">The collection of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found works.
    /// </returns>
    public Task<WorkDto[]> GetWorksAsync(IEnumerable<Guid> workIds);

    /// <summary>
    /// Asynchronously gets works filtered by a collection processor.
    /// </summary>
    /// <param name="collectionProcessor">The collection processor to filter works.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found works.
    /// </returns>
    public Task<WorkDto[]> GetWorksAsync(EntityCollectionProcessor<WorkDto> collectionProcessor);

    /// <summary>
    /// Asynchronously gets works by a work page request.
    /// </summary>
    /// <param name="workRequest">The work page request configuration.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all works corresponding to the request configuration.
    /// </returns>
    public Task<PageResponseDto<WorkDto>> GetWorksAsync(WorkRequestDto workRequest);

    /// <summary>
    /// Asynchronously gets all work relationships by a work's unique identifier.
    /// </summary>
    /// <param name="workId">The work's unique identifier.</param>
    /// <param name="includeReverseRelationships">A boolean value specifying whether reverse relationships should be included.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all work relationships.
    /// </returns>
    public Task<WorkRelationshipDto[]> GetWorkRelationshipsAsync(Guid workId, bool includeReverseRelationships = false);

    /// <summary>
    /// Asynchronously gets all work-to-product relationships by a work's unique identifier.
    /// </summary>
    /// <param name="workId">The work's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all work-to-product relationships.
    /// </returns>
    public Task<WorkToProductRelationshipDto[]> GetWorkToProductRelationshipsAsync(Guid workId);

    /// <summary>
    /// Asynchronously creates a new work.
    /// </summary>
    /// <param name="work">The work to create in the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created work with properties like <see cref="WorkDto.Id" /> set.
    /// </returns>
    public Task<WorkDto> CreateWorkAsync(WorkDto work);

    /// <summary>
    /// Asynchronously updates an existing work.
    /// </summary>
    /// <param name="work">The work to update in the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the work was found and updated.
    /// </returns>
    public Task<bool> UpdateWorkAsync(WorkDto work);

    /// <summary>
    /// Asynchronously deletes an existing work.
    /// </summary>
    /// <param name="workId">The unique identifier of the work to delete from the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the work was found and deleted.
    /// </returns>
    public Task<bool> DeleteWorkAsync(Guid workId);
}
