using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Controllers;

/// <summary>
/// Represents a Web API controller for the <see cref="ReleaseGroup" /> entity.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class ReleaseGroupController : ControllerBase
{
    private readonly IReleaseGroupService m_releaseGroupService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReleaseGroupController" /> type using the specified services.
    /// </summary>
    /// <param name="releaseGroupService">The release group service.</param>
    public ReleaseGroupController(IReleaseGroupService releaseGroupService)
    {
        m_releaseGroupService = releaseGroupService;
    }

    /// <summary>
    /// Asynchronously gets a release group by its unique identifier.
    /// </summary>
    /// <param name="releaseGroupId">The release group's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the release group found or <see langword="null" />.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReleaseGroup>> GetReleaseGroupAsync([Required][FromQuery] Guid releaseGroupId)
    {
        ReleaseGroup? releaseGroup = await m_releaseGroupService.GetReleaseGroupAsync(releaseGroupId);
        return releaseGroup is not null
            ? (ActionResult<ReleaseGroup>)Ok(releaseGroup)
            : (ActionResult<ReleaseGroup>)NotFound();
    }

    /// <summary>
    /// Asynchronously gets release groups by an array of unique identifiers.
    /// </summary>
    /// <param name="releaseGroupIds">The array of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found release groups.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReleaseGroup[]>> GetReleaseGroupsAsync([Required][FromQuery] Guid[] releaseGroupIds)
    {
        ReleaseGroup[] releaseGroups = await m_releaseGroupService.GetReleaseGroupsAsync(releaseGroupIds);
        return Ok(releaseGroups);
    }

    /// <summary>
    /// Asynchronously gets all release groups.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release groups.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReleaseGroup[]>> GetAllReleaseGroupsAsync()
    {
        ReleaseGroup[] releaseGroups = await m_releaseGroupService.GetReleaseGroupsAsync();
        return Ok(releaseGroups);
    }

    /// <summary>
    /// Asynchronously gets release groups by a release group page request.
    /// </summary>
    /// <param name="pageSize">The page size.</param>
    /// <param name="pageIndex">The page index.</param>
    /// <param name="title">The filter value for the <see cref="ReleaseGroup.Title" /> property.</param>
    /// <param name="enabled">The filter value for the <see cref="ReleaseGroup.Enabled" /> property.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all release groups corresponding to the request configuration.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReleaseGroupPageResponse>> GetPagedReleaseGroupsAsync([Required][FromQuery] int pageSize, [Required][FromQuery] int pageIndex, [FromQuery] string? title, [FromQuery] bool? enabled)
    {
        var releaseGroupRequest = new ReleaseGroupRequest()
        {
            PageSize = pageSize,
            PageIndex = pageIndex,
            Title = title,
            Enabled = enabled,
        };

        ReleaseGroupPageResponse pageResponse = await m_releaseGroupService.GetReleaseGroupsAsync(releaseGroupRequest);
        return Ok(pageResponse);
    }

    /// <summary>
    /// Asynchronously creates a new release group.
    /// </summary>
    /// <param name="releaseGroup">The release group to create.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created release group with properties like <see cref="ReleaseGroup.Id" /> set.
    /// </returns>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReleaseGroup>> CreateReleaseGroupAsync([Required][FromBody] ReleaseGroup releaseGroup)
    {
        ReleaseGroup createdReleaseGroup = await m_releaseGroupService.CreateReleaseGroupAsync(releaseGroup);
        return Ok(createdReleaseGroup);
    }

    /// <summary>
    /// Asynchronously updates an existing release group.
    /// </summary>
    /// <param name="releaseGroup">The release group to update.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateReleaseGroupAsync([Required][FromBody] ReleaseGroup releaseGroup)
    {
        var result = await m_releaseGroupService.UpdateReleaseGroupAsync(releaseGroup);
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Asynchronously deletes an existing release group.
    /// </summary>
    /// <param name="releaseGroupId">The unique identifier of the release group to delete.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteReleaseGroupAsync([Required][FromQuery] Guid releaseGroupId)
    {
        var result = await m_releaseGroupService.DeleteReleaseGroupAsync(releaseGroupId);
        return result ? Ok() : NotFound();
    }
}
