using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

/// <summary>
/// Defines a set of members a provider-specific release group repository should implement.
/// </summary>
public interface IReleaseGroupRepository
{
    /// <summary>
    /// Asynchronously gets a release group by its unique identifier.
    /// </summary>
    /// <param name="releaseGroupId">The release group's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the release group found or <see langword="null" />.
    /// </returns>
    public Task<ReleaseGroupDto?> GetReleaseGroupAsync(Guid releaseGroupId);

    /// <summary>
    /// Asynchronously gets all release groups.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release groups.
    /// </returns>
    public Task<ReleaseGroupDto[]> GetReleaseGroupsAsync();

    /// <summary>
    /// Asynchronously gets release groups by a collection of unique identifiers.
    /// </summary>
    /// <param name="releaseGroupIds">The collection of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found release groups.
    /// </returns>
    public Task<ReleaseGroupDto[]> GetReleaseGroupsAsync(IEnumerable<Guid> releaseGroupIds);

    /// <summary>
    /// Asynchronously gets release groups filtered by a collection processor.
    /// </summary>
    /// <param name="collectionProcessor">The collection processor to filter release groups.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found release groups.
    /// </returns>
    public Task<ReleaseGroupDto[]> GetReleaseGroupsAsync(EntityCollectionProcessor<ReleaseGroupDto> collectionProcessor);

    /// <summary>
    /// Asynchronously gets release groups by a release group page request.
    /// </summary>
    /// <param name="releaseGroupRequest">The release group page request configuration.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release groups corresponding to the request configuration.
    /// </returns>
    public Task<PageResponseDto<ReleaseGroupDto>> GetReleaseGroupsAsync(ReleaseGroupRequestDto releaseGroupRequest);

    /// <summary>
    /// Asynchronously gets all release group relationships by a release group's unique identifier.
    /// </summary>
    /// <param name="releaseGroupId">The release group's unique identifier.</param>
    /// <param name="includeReverseRelationships">A boolean value specifying whether reverse relationships should be included.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release group relationships.
    /// </returns>
    public Task<ReleaseGroupRelationshipDto[]> GetReleaseGroupRelationshipsAsync(Guid releaseGroupId, bool includeReverseRelationships = false);

    /// <summary>
    /// Asynchronously creates a new release group.
    /// </summary>
    /// <param name="releaseGroup">The release group to create in the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created release group with properties like <see cref="ReleaseGroupDto.Id" /> set.
    /// </returns>
    public Task<ReleaseGroupDto> CreateReleaseGroupAsync(ReleaseGroupDto releaseGroup);

    /// <summary>
    /// Asynchronously updates an existing release group.
    /// </summary>
    /// <param name="releaseGroup">The release group to update in the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the release group was found and updated.
    /// </returns>
    public Task<bool> UpdateReleaseGroupAsync(ReleaseGroupDto releaseGroup);

    /// <summary>
    /// Asynchronously deletes an existing release group.
    /// </summary>
    /// <param name="releaseGroupId">The unique identifier of the release group to delete from the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the release group was found and deleted.
    /// </returns>
    public Task<bool> DeleteReleaseGroupAsync(Guid releaseGroupId);
}
