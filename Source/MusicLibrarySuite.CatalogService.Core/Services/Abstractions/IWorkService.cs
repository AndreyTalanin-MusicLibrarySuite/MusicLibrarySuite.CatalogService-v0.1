using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services.Abstractions;

/// <summary>
/// Defines a set of members a work service should implement.
/// </summary>
public interface IWorkService
{
    /// <summary>
    /// Asynchronously gets a work by its unique identifier.
    /// </summary>
    /// <param name="workId">The work's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the work found or <see langword="null" />.
    /// </returns>
    public Task<Work?> GetWorkAsync(Guid workId);

    /// <summary>
    /// Asynchronously gets all works.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all works.
    /// </returns>
    public Task<Work[]> GetWorksAsync();

    /// <summary>
    /// Asynchronously gets works by a collection of unique identifiers.
    /// </summary>
    /// <param name="workIds">The collection of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found works.
    /// </returns>
    public Task<Work[]> GetWorksAsync(IEnumerable<Guid> workIds);

    /// <summary>
    /// Asynchronously gets works by a work page request.
    /// </summary>
    /// <param name="workRequest">The work page request configuration.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all works corresponding to the request configuration.
    /// </returns>
    public Task<WorkPageResponse> GetWorksAsync(WorkRequest workRequest);

    /// <summary>
    /// Asynchronously gets all work relationships by a work's unique identifier.
    /// </summary>
    /// <param name="workId">The work's unique identifier.</param>
    /// <param name="includeReverseRelationships">A boolean value specifying whether reverse relationships should be included.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all work relationships.
    /// </returns>
    public Task<WorkRelationship[]> GetWorkRelationshipsAsync(Guid workId, bool includeReverseRelationships = false);

    /// <summary>
    /// Asynchronously gets all work-to-product relationships by a work's unique identifier.
    /// </summary>
    /// <param name="workId">The work's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all work-to-product relationships.
    /// </returns>
    public Task<WorkToProductRelationship[]> GetWorkToProductRelationshipsAsync(Guid workId);

    /// <summary>
    /// Asynchronously creates a new work.
    /// </summary>
    /// <param name="work">The work to create.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created work with properties like <see cref="Work.Id" /> set.
    /// </returns>
    public Task<Work> CreateWorkAsync(Work work);

    /// <summary>
    /// Asynchronously updates an existing work.
    /// </summary>
    /// <param name="work">The work to update.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the work was found and updated.
    /// </returns>
    public Task<bool> UpdateWorkAsync(Work work);

    /// <summary>
    /// Asynchronously deletes an existing work.
    /// </summary>
    /// <param name="workId">The unique identifier of the work to delete.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the work was found and deleted.
    /// </returns>
    public Task<bool> DeleteWorkAsync(Guid workId);
}
