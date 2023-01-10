using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services.Abstractions;

/// <summary>
/// Defines a set of members a release service should implement.
/// </summary>
public interface IReleaseService
{
    /// <summary>
    /// Asynchronously gets a release by its unique identifier.
    /// </summary>
    /// <param name="releaseId">The release's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the release found or <see langword="null" />.
    /// </returns>
    public Task<Release?> GetReleaseAsync(Guid releaseId);

    /// <summary>
    /// Asynchronously gets all releases.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all releases.
    /// </returns>
    public Task<Release[]> GetReleasesAsync();

    /// <summary>
    /// Asynchronously gets releases by a collection of unique identifiers.
    /// </summary>
    /// <param name="releaseIds">The collection of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found releases.
    /// </returns>
    public Task<Release[]> GetReleasesAsync(IEnumerable<Guid> releaseIds);

    /// <summary>
    /// Asynchronously gets releases by a release page request.
    /// </summary>
    /// <param name="releaseRequest">The release page request configuration.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all releases corresponding to the request configuration.
    /// </returns>
    public Task<ReleasePageResponse> GetReleasesAsync(ReleaseRequest releaseRequest);

    /// <summary>
    /// Asynchronously creates a new release.
    /// </summary>
    /// <param name="release">The release to create.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created release with properties like <see cref="Release.Id" /> set.
    /// </returns>
    public Task<Release> CreateReleaseAsync(Release release);

    /// <summary>
    /// Asynchronously updates an existing release.
    /// </summary>
    /// <param name="release">The release to update.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the release was found and updated.
    /// </returns>
    public Task<bool> UpdateReleaseAsync(Release release);

    /// <summary>
    /// Asynchronously deletes an existing release.
    /// </summary>
    /// <param name="releaseId">The unique identifier of the release to delete.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the release was found and deleted.
    /// </returns>
    public Task<bool> DeleteReleaseAsync(Guid releaseId);
}
