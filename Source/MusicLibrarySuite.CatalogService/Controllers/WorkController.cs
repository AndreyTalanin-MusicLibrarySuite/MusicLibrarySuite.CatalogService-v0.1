using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Controllers;

/// <summary>
/// Represents a Web API controller for the <see cref="Work" /> entity.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class WorkController : ControllerBase
{
    private readonly IWorkService m_workService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkController" /> type using the specified services.
    /// </summary>
    /// <param name="workService">The work service.</param>
    public WorkController(IWorkService workService)
    {
        m_workService = workService;
    }

    /// <summary>
    /// Asynchronously gets a work by its unique identifier.
    /// </summary>
    /// <param name="workId">The work's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the work found or <see langword="null" />.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Work>> GetWorkAsync([Required][FromQuery] Guid workId)
    {
        Work? work = await m_workService.GetWorkAsync(workId);
        return work is not null
            ? (ActionResult<Work>)Ok(work)
            : (ActionResult<Work>)NotFound();
    }

    /// <summary>
    /// Asynchronously gets works by an array of unique identifiers.
    /// </summary>
    /// <param name="workIds">The array of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found works.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Work[]>> GetWorksAsync([Required][FromQuery] Guid[] workIds)
    {
        Work[] works = await m_workService.GetWorksAsync(workIds);
        return Ok(works);
    }

    /// <summary>
    /// Asynchronously gets all works.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all works.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Work[]>> GetAllWorksAsync()
    {
        Work[] works = await m_workService.GetWorksAsync();
        return Ok(works);
    }

    /// <summary>
    /// Asynchronously gets works by a work page request.
    /// </summary>
    /// <param name="pageSize">The page size.</param>
    /// <param name="pageIndex">The page index.</param>
    /// <param name="title">The filter value for the <see cref="Work.Title" /> property.</param>
    /// <param name="enabled">The filter value for the <see cref="Work.Enabled" /> property.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all works corresponding to the request configuration.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkPageResponse>> GetPagedWorksAsync([Required][FromQuery] int pageSize, [Required][FromQuery] int pageIndex, [FromQuery] string? title, [FromQuery] bool? enabled)
    {
        var workRequest = new WorkRequest()
        {
            PageSize = pageSize,
            PageIndex = pageIndex,
            Title = title,
            Enabled = enabled,
        };

        WorkPageResponse pageResponse = await m_workService.GetWorksAsync(workRequest);
        return Ok(pageResponse);
    }

    /// <summary>
    /// Asynchronously gets all work relationships by a work's unique identifier.
    /// </summary>
    /// <param name="workId">The work's unique identifier.</param>
    /// <param name="includeReverseRelationships">A boolean value specifying whether reverse relationships should be included.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all work relationships.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkRelationship[]>> GetWorkRelationshipsAsync([Required][FromQuery] Guid workId, [FromQuery] bool includeReverseRelationships)
    {
        WorkRelationship[] workRelationships = await m_workService.GetWorkRelationshipsAsync(workId, includeReverseRelationships);
        return Ok(workRelationships);
    }

    /// <summary>
    /// Asynchronously gets all work-to-product relationships by a work's unique identifier.
    /// </summary>
    /// <param name="workId">The work's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all work-to-product relationships.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkToProductRelationship[]>> GetWorkToProductRelationshipsAsync([Required][FromQuery] Guid workId)
    {
        WorkToProductRelationship[] workToProductRelationships = await m_workService.GetWorkToProductRelationshipsAsync(workId);
        return Ok(workToProductRelationships);
    }

    /// <summary>
    /// Asynchronously gets all work-to-product relationships by a product's unique identifier.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all work-to-product relationships.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkToProductRelationship[]>> GetWorkToProductRelationshipsByProductAsync([Required][FromQuery] Guid productId)
    {
        WorkToProductRelationship[] workToProductRelationships = await m_workService.GetWorkToProductRelationshipsByProductAsync(productId);
        return Ok(workToProductRelationships);
    }

    /// <summary>
    /// Asynchronously creates a new work.
    /// </summary>
    /// <param name="work">The work to create.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created work with properties like <see cref="Work.Id" /> set.
    /// </returns>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Work>> CreateWorkAsync([Required][FromBody] Work work)
    {
        Work createdWork = await m_workService.CreateWorkAsync(work);
        return Ok(createdWork);
    }

    /// <summary>
    /// Asynchronously updates an existing work.
    /// </summary>
    /// <param name="work">The work to update.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateWorkAsync([Required][FromBody] Work work)
    {
        var result = await m_workService.UpdateWorkAsync(work);
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Asynchronously updates order of existing work-to-product relationships.
    /// </summary>
    /// <param name="workToProductRelationships">A collection of work-to-product relationships to reorder.</param>
    /// <param name="useReferenceOrder">A value indicating whether the reference order should be used.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateWorkToProductRelationshipsOrderAsync([Required][FromBody] WorkToProductRelationship[] workToProductRelationships, [FromQuery] bool? useReferenceOrder)
    {
        var result = await m_workService.UpdateWorkToProductRelationshipsOrderAsync(workToProductRelationships, useReferenceOrder == true);
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Asynchronously deletes an existing work.
    /// </summary>
    /// <param name="workId">The unique identifier of the work to delete.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteWorkAsync([Required][FromQuery] Guid workId)
    {
        var result = await m_workService.DeleteWorkAsync(workId);
        return result ? Ok() : NotFound();
    }
}
