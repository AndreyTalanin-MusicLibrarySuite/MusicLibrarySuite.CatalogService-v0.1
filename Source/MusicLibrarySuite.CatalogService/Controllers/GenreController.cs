using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Controllers;

/// <summary>
/// Represents a Web API controller for the <see cref="Genre" /> entity.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class GenreController : ControllerBase
{
    private readonly IGenreService m_genreService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreController" /> type using the specified services.
    /// </summary>
    /// <param name="genreService">The genre service.</param>
    public GenreController(IGenreService genreService)
    {
        m_genreService = genreService;
    }

    /// <summary>
    /// Asynchronously gets a genre by its unique identifier.
    /// </summary>
    /// <param name="genreId">The genre's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the genre found or <see langword="null" />.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Genre>> GetGenreAsync([Required][FromQuery] Guid genreId)
    {
        Genre? genre = await m_genreService.GetGenreAsync(genreId);
        return genre is not null
            ? (ActionResult<Genre>)Ok(genre)
            : (ActionResult<Genre>)NotFound();
    }

    /// <summary>
    /// Asynchronously gets genres by an array of unique identifiers.
    /// </summary>
    /// <param name="genreIds">The array of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found genres.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Genre[]>> GetGenresAsync([Required][FromQuery] Guid[] genreIds)
    {
        Genre[] genres = await m_genreService.GetGenresAsync(genreIds);
        return Ok(genres);
    }

    /// <summary>
    /// Asynchronously gets all genres.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all genres.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Genre[]>> GetAllGenresAsync()
    {
        Genre[] genres = await m_genreService.GetGenresAsync();
        return Ok(genres);
    }

    /// <summary>
    /// Asynchronously gets genres by a genre page request.
    /// </summary>
    /// <param name="pageSize">The page size.</param>
    /// <param name="pageIndex">The page index.</param>
    /// <param name="name">The filter value for the <see cref="Genre.Name" /> property.</param>
    /// <param name="enabled">The filter value for the <see cref="Genre.Enabled" /> property.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all genres corresponding to the request configuration.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GenrePageResponse>> GetPagedGenresAsync([Required][FromQuery] int pageSize, [Required][FromQuery] int pageIndex, [FromQuery] string? name, [FromQuery] bool? enabled)
    {
        var genreRequest = new GenreRequest()
        {
            PageSize = pageSize,
            PageIndex = pageIndex,
            Name = name,
            Enabled = enabled,
        };

        GenrePageResponse pageResponse = await m_genreService.GetGenresAsync(genreRequest);
        return Ok(pageResponse);
    }

    /// <summary>
    /// Asynchronously creates a new genre.
    /// </summary>
    /// <param name="genre">The genre to create.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created genre with properties like <see cref="Genre.Id" /> set.
    /// </returns>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Genre>> CreateGenreAsync([Required][FromBody] Genre genre)
    {
        Genre createdGenre = await m_genreService.CreateGenreAsync(genre);
        return Ok(createdGenre);
    }

    /// <summary>
    /// Asynchronously updates an existing genre.
    /// </summary>
    /// <param name="genre">The genre to update.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateGenreAsync([Required][FromBody] Genre genre)
    {
        var result = await m_genreService.UpdateGenreAsync(genre);
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Asynchronously deletes an existing genre.
    /// </summary>
    /// <param name="genreId">The unique identifier of the genre to delete.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteGenreAsync([Required][FromQuery] Guid genreId)
    {
        var result = await m_genreService.DeleteGenreAsync(genreId);
        return result ? Ok() : NotFound();
    }
}
