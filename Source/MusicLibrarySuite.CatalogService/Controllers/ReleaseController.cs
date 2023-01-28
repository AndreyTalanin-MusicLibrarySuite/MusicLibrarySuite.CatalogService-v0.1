using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Controllers;

/// <summary>
/// Represents a Web API controller for the <see cref="Release" /> entity.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class ReleaseController : ControllerBase
{
    private readonly IReleaseService m_releaseService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReleaseController" /> type using the specified services.
    /// </summary>
    /// <param name="releaseService">The release service.</param>
    public ReleaseController(IReleaseService releaseService)
    {
        m_releaseService = releaseService;
    }

    /// <summary>
    /// Asynchronously gets a release by its unique identifier.
    /// </summary>
    /// <param name="releaseId">The release's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the release found or <see langword="null" />.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Release>> GetReleaseAsync([Required][FromQuery] Guid releaseId)
    {
        Release? release = await m_releaseService.GetReleaseAsync(releaseId);
        return release is not null
            ? (ActionResult<Release>)Ok(release)
            : (ActionResult<Release>)NotFound();
    }

    /// <summary>
    /// Asynchronously gets releases by an array of unique identifiers.
    /// </summary>
    /// <param name="releaseIds">The array of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found releases.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Release[]>> GetReleasesAsync([Required][FromQuery] Guid[] releaseIds)
    {
        Release[] releases = await m_releaseService.GetReleasesAsync(releaseIds);
        return Ok(releases);
    }

    /// <summary>
    /// Asynchronously gets all releases.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all releases.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Release[]>> GetAllReleasesAsync()
    {
        Release[] releases = await m_releaseService.GetReleasesAsync();
        return Ok(releases);
    }

    /// <summary>
    /// Asynchronously gets releases by a release page request.
    /// </summary>
    /// <param name="pageSize">The page size.</param>
    /// <param name="pageIndex">The page index.</param>
    /// <param name="title">The filter value for the <see cref="Release.Title" /> property.</param>
    /// <param name="enabled">The filter value for the <see cref="Release.Enabled" /> property.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all releases corresponding to the request configuration.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReleasePageResponse>> GetPagedReleasesAsync([Required][FromQuery] int pageSize, [Required][FromQuery] int pageIndex, [FromQuery] string? title, [FromQuery] bool? enabled)
    {
        var releaseRequest = new ReleaseRequest()
        {
            PageSize = pageSize,
            PageIndex = pageIndex,
            Title = title,
            Enabled = enabled,
        };

        ReleasePageResponse pageResponse = await m_releaseService.GetReleasesAsync(releaseRequest);
        return Ok(pageResponse);
    }

    /// <summary>
    /// Asynchronously gets all release relationships by a release's unique identifier.
    /// </summary>
    /// <param name="releaseId">The release's unique identifier.</param>
    /// <param name="includeReverseRelationships">A boolean value specifying whether reverse relationships should be included.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release relationships.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReleaseRelationship[]>> GetReleaseRelationshipsAsync([Required][FromQuery] Guid releaseId, [FromQuery] bool includeReverseRelationships)
    {
        ReleaseRelationship[] releaseRelationships = await m_releaseService.GetReleaseRelationshipsAsync(releaseId, includeReverseRelationships);
        return Ok(releaseRelationships);
    }

    /// <summary>
    /// Asynchronously gets all release-to-product relationships by a release's unique identifier.
    /// </summary>
    /// <param name="releaseId">The release's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-to-product relationships.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReleaseToProductRelationship[]>> GetReleaseToProductRelationshipsAsync([Required][FromQuery] Guid releaseId)
    {
        ReleaseToProductRelationship[] releaseToProductRelationships = await m_releaseService.GetReleaseToProductRelationshipsAsync(releaseId);
        return Ok(releaseToProductRelationships);
    }

    /// <summary>
    /// Asynchronously gets all release-to-product relationships by a product's unique identifier.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-to-product relationships.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReleaseToProductRelationship[]>> GetReleaseToProductRelationshipsByProductAsync([Required][FromQuery] Guid productId)
    {
        ReleaseToProductRelationship[] releaseToProductRelationships = await m_releaseService.GetReleaseToProductRelationshipsByProductAsync(productId);
        return Ok(releaseToProductRelationships);
    }

    /// <summary>
    /// Asynchronously gets all release-to-release-group relationships by a release's unique identifier.
    /// </summary>
    /// <param name="releaseId">The release's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-to-release-group relationships.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReleaseToReleaseGroupRelationship[]>> GetReleaseToReleaseGroupRelationshipsAsync([Required][FromQuery] Guid releaseId)
    {
        ReleaseToReleaseGroupRelationship[] releaseToReleaseGroupRelationships = await m_releaseService.GetReleaseToReleaseGroupRelationshipsAsync(releaseId);
        return Ok(releaseToReleaseGroupRelationships);
    }

    /// <summary>
    /// Asynchronously gets all release-to-release-group relationships by a release group's unique identifier.
    /// </summary>
    /// <param name="releaseGroupId">The release group's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release-to-release-group relationships.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReleaseToReleaseGroupRelationship[]>> GetReleaseToReleaseGroupRelationshipsByReleaseGroupAsync([Required][FromQuery] Guid releaseGroupId)
    {
        ReleaseToReleaseGroupRelationship[] releaseToReleaseGroupRelationships = await m_releaseService.GetReleaseToReleaseGroupRelationshipsByReleaseGroupAsync(releaseGroupId);
        return Ok(releaseToReleaseGroupRelationships);
    }

    /// <summary>
    /// Asynchronously creates a new release.
    /// </summary>
    /// <param name="release">The release to create.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created release with properties like <see cref="Release.Id" /> set.
    /// </returns>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Release>> CreateReleaseAsync([Required][FromBody] Release release)
    {
        Release createdRelease = await m_releaseService.CreateReleaseAsync(release);
        return Ok(createdRelease);
    }

    /// <summary>
    /// Asynchronously updates an existing release.
    /// </summary>
    /// <param name="release">The release to update.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateReleaseAsync([Required][FromBody] Release release)
    {
        var result = await m_releaseService.UpdateReleaseAsync(release);
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Asynchronously updates order of existing release-to-product relationships.
    /// </summary>
    /// <param name="releaseToProductRelationships">A collection of release-to-product relationships to reorder.</param>
    /// <param name="useReferenceOrder">A value indicating whether the reference order should be used.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateReleaseToProductRelationshipsOrderAsync([Required][FromBody] ReleaseToProductRelationship[] releaseToProductRelationships, [FromQuery] bool? useReferenceOrder)
    {
        var result = await m_releaseService.UpdateReleaseToProductRelationshipsOrderAsync(releaseToProductRelationships, useReferenceOrder == true);
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Asynchronously deletes an existing release.
    /// </summary>
    /// <param name="releaseId">The unique identifier of the release to delete.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteReleaseAsync([Required][FromQuery] Guid releaseId)
    {
        var result = await m_releaseService.DeleteReleaseAsync(releaseId);
        return result ? Ok() : NotFound();
    }
}
