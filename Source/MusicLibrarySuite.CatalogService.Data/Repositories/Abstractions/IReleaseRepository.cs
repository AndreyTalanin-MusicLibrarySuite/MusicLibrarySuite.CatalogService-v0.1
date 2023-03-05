using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

/// <summary>
/// Defines a set of members a provider-specific release repository should implement.
/// </summary>
public interface IReleaseRepository
{
    /// <summary>
    /// Asynchronously gets a release by its unique identifier.
    /// </summary>
    /// <param name="releaseId">The release's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the release found or <see langword="null" />.
    /// </returns>
    public Task<ReleaseDto?> GetReleaseAsync(Guid releaseId);

    /// <summary>
    /// Asynchronously gets all releases.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all releases.
    /// </returns>
    public Task<ReleaseDto[]> GetReleasesAsync();

    /// <summary>
    /// Asynchronously gets releases by a collection of unique identifiers.
    /// </summary>
    /// <param name="releaseIds">The collection of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found releases.
    /// </returns>
    public Task<ReleaseDto[]> GetReleasesAsync(IEnumerable<Guid> releaseIds);

    /// <summary>
    /// Asynchronously gets releases filtered by a collection processor.
    /// </summary>
    /// <param name="collectionProcessor">The collection processor to filter releases.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found releases.
    /// </returns>
    public Task<ReleaseDto[]> GetReleasesAsync(EntityCollectionProcessor<ReleaseDto> collectionProcessor);

    /// <summary>
    /// Asynchronously gets releases by a release page request.
    /// </summary>
    /// <param name="releaseRequest">The release page request configuration.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all releases corresponding to the request configuration.
    /// </returns>
    public Task<PageResponseDto<ReleaseDto>> GetReleasesAsync(ReleaseRequestDto releaseRequest);

    /// <summary>
    /// Asynchronously gets all release relationships by a release's unique identifier.
    /// </summary>
    /// <param name="releaseId">The release's unique identifier.</param>
    /// <param name="includeReverseRelationships">A boolean value specifying whether reverse relationships should be included.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release relationships.
    /// </returns>
    public Task<ReleaseRelationshipDto[]> GetReleaseRelationshipsAsync(Guid releaseId, bool includeReverseRelationships = false);

    /// <summary>
    /// Asynchronously gets all release-to-product relationships by a release's unique identifier.
    /// </summary>
    /// <param name="releaseId">The release's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-to-product relationships.
    /// </returns>
    public Task<ReleaseToProductRelationshipDto[]> GetReleaseToProductRelationshipsAsync(Guid releaseId);

    /// <summary>
    /// Asynchronously gets all release-to-product relationships by a product's unique identifier.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-to-product relationships.
    /// </returns>
    public Task<ReleaseToProductRelationshipDto[]> GetReleaseToProductRelationshipsByProductAsync(Guid productId);

    /// <summary>
    /// Asynchronously gets all release-to-release-group relationships by a release's unique identifier.
    /// </summary>
    /// <param name="releaseId">The release's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-to-release-group relationships.
    /// </returns>
    public Task<ReleaseToReleaseGroupRelationshipDto[]> GetReleaseToReleaseGroupRelationshipsAsync(Guid releaseId);

    /// <summary>
    /// Asynchronously gets all release-to-release-group relationships by a release group's unique identifier.
    /// </summary>
    /// <param name="releaseGroupId">The release group's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-to-release-group relationships.
    /// </returns>
    public Task<ReleaseToReleaseGroupRelationshipDto[]> GetReleaseToReleaseGroupRelationshipsByReleaseGroupAsync(Guid releaseGroupId);

    /// <summary>
    /// Asynchronously gets all release-media-to-product relationships by a release's unique identifier.
    /// </summary>
    /// <param name="releaseId">The release's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-media-to-product relationships.
    /// </returns>
    public Task<ReleaseMediaToProductRelationshipDto[]> GetReleaseMediaToProductRelationshipsAsync(Guid releaseId);

    /// <summary>
    /// Asynchronously gets all release-media-to-product relationships by a product's unique identifier.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-media-to-product relationships.
    /// </returns>
    public Task<ReleaseMediaToProductRelationshipDto[]> GetReleaseMediaToProductRelationshipsByProductAsync(Guid productId);

    /// <summary>
    /// Asynchronously gets all release-track-to-product relationships by a release's unique identifier.
    /// </summary>
    /// <param name="releaseId">The release's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-track-to-product relationships.
    /// </returns>
    public Task<ReleaseTrackToProductRelationshipDto[]> GetReleaseTrackToProductRelationshipsAsync(Guid releaseId);

    /// <summary>
    /// Asynchronously gets all release-track-to-product relationships by a product's unique identifier.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-track-to-product relationships.
    /// </returns>
    public Task<ReleaseTrackToProductRelationshipDto[]> GetReleaseTrackToProductRelationshipsByProductAsync(Guid productId);

    /// <summary>
    /// Asynchronously gets all release-track-to-work relationships by a release's unique identifier.
    /// </summary>
    /// <param name="releaseId">The release's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-track-to-work relationships.
    /// </returns>
    public Task<ReleaseTrackToWorkRelationshipDto[]> GetReleaseTrackToWorkRelationshipsAsync(Guid releaseId);

    /// <summary>
    /// Asynchronously gets all release-track-to-work relationships by a work's unique identifier.
    /// </summary>
    /// <param name="workId">The work's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-track-to-work relationships.
    /// </returns>
    public Task<ReleaseTrackToWorkRelationshipDto[]> GetReleaseTrackToWorkRelationshipsByWorkAsync(Guid workId);

    /// <summary>
    /// Asynchronously creates a new release.
    /// </summary>
    /// <param name="release">The release to create in the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created release with properties like <see cref="ReleaseDto.Id" /> set.
    /// </returns>
    public Task<ReleaseDto> CreateReleaseAsync(ReleaseDto release);

    /// <summary>
    /// Asynchronously updates an existing release.
    /// </summary>
    /// <param name="release">The release to update in the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the release was found and updated.
    /// </returns>
    public Task<bool> UpdateReleaseAsync(ReleaseDto release);

    /// <summary>
    /// Asynchronously updates order of existing release-to-product relationships.
    /// </summary>
    /// <param name="releaseToProductRelationships">A collection of release-to-product relationships to reorder.</param>
    /// <param name="useReferenceOrder">A value indicating whether the <see cref="ReleaseToProductRelationshipDto.ReferenceOrder" /> property should be used.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether any release-to-product relationship was found and updated.
    /// </returns>
    public Task<bool> UpdateReleaseToProductRelationshipsOrderAsync(ReleaseToProductRelationshipDto[] releaseToProductRelationships, bool useReferenceOrder = false);

    /// <summary>
    /// Asynchronously updates order of existing release-to-release-group relationships.
    /// </summary>
    /// <param name="releaseToReleaseGroupRelationships">A collection of release-to-release-group relationships to reorder.</param>
    /// <param name="useReferenceOrder">A value indicating whether the <see cref="ReleaseToReleaseGroupRelationshipDto.ReferenceOrder" /> property should be used.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether any release-to-release-group relationship was found and updated.
    /// </returns>
    public Task<bool> UpdateReleaseToReleaseGroupRelationshipsOrderAsync(ReleaseToReleaseGroupRelationshipDto[] releaseToReleaseGroupRelationships, bool useReferenceOrder = false);

    /// <summary>
    /// Asynchronously updates order of existing release-media-to-product relationships.
    /// </summary>
    /// <param name="releaseMediaToProductRelationships">A collection of release-media-to-product relationships to reorder.</param>
    /// <param name="useReferenceOrder">A value indicating whether the <see cref="ReleaseMediaToProductRelationshipDto.ReferenceOrder" /> property should be used.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether any release-media-to-product relationship was found and updated.
    /// </returns>
    public Task<bool> UpdateReleaseMediaToProductRelationshipsOrderAsync(ReleaseMediaToProductRelationshipDto[] releaseMediaToProductRelationships, bool useReferenceOrder = false);

    /// <summary>
    /// Asynchronously updates order of existing release-track-to-product relationships.
    /// </summary>
    /// <param name="releaseTrackToProductRelationships">A collection of release-track-to-product relationships to reorder.</param>
    /// <param name="useReferenceOrder">A value indicating whether the <see cref="ReleaseTrackToProductRelationshipDto.ReferenceOrder" /> property should be used.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether any release-track-to-product relationship was found and updated.
    /// </returns>
    public Task<bool> UpdateReleaseTrackToProductRelationshipsOrderAsync(ReleaseTrackToProductRelationshipDto[] releaseTrackToProductRelationships, bool useReferenceOrder = false);

    /// <summary>
    /// Asynchronously updates order of existing release-track-to-work relationships.
    /// </summary>
    /// <param name="releaseTrackToWorkRelationships">A collection of release-track-to-work relationships to reorder.</param>
    /// <param name="useReferenceOrder">A value indicating whether the <see cref="ReleaseTrackToWorkRelationshipDto.ReferenceOrder" /> property should be used.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether any release-track-to-work relationship was found and updated.
    /// </returns>
    public Task<bool> UpdateReleaseTrackToWorkRelationshipsOrderAsync(ReleaseTrackToWorkRelationshipDto[] releaseTrackToWorkRelationships, bool useReferenceOrder = false);

    /// <summary>
    /// Asynchronously deletes an existing release.
    /// </summary>
    /// <param name="releaseId">The unique identifier of the release to delete from the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the release was found and deleted.
    /// </returns>
    public Task<bool> DeleteReleaseAsync(Guid releaseId);
}
