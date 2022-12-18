using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MusicLibrarySuite.CatalogService.Core.Services;
using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Controllers;

/// <summary>
/// Represents a Web API controller for the <see cref="Artist" /> entity.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class ArtistController : ControllerBase
{
    private readonly IArtistService m_artistService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistController" /> type using the specified services.
    /// </summary>
    /// <param name="artistService">The artist service.</param>
    public ArtistController(IArtistService artistService)
    {
        m_artistService = artistService;
    }

    /// <summary>
    /// Asynchronously gets an artist by its unique identifier.
    /// </summary>
    /// <param name="artistId">The artist's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the artist found or <see langword="null" />.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Artist>> GetArtistAsync([Required][FromQuery] Guid artistId)
    {
        Artist? artist = await m_artistService.GetArtistAsync(artistId);
        return artist is not null
            ? (ActionResult<Artist>)Ok(artist)
            : (ActionResult<Artist>)NotFound();
    }

    /// <summary>
    /// Asynchronously gets artists by an array of unique identifiers.
    /// </summary>
    /// <param name="artistIds">The array of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found artists.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Artist[]>> GetArtistsAsync([Required][FromQuery] Guid[] artistIds)
    {
        Artist[] artists = await m_artistService.GetArtistsAsync(artistIds);
        return Ok(artists);
    }

    /// <summary>
    /// Asynchronously gets all artists.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all artists.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Artist[]>> GetAllArtistsAsync()
    {
        Artist[] artists = await m_artistService.GetArtistsAsync();
        return Ok(artists);
    }

    /// <summary>
    /// Asynchronously gets artists by an artist page request.
    /// </summary>
    /// <param name="pageSize">The page size.</param>
    /// <param name="pageIndex">The page index.</param>
    /// <param name="name">The filter value for the <see cref="Artist.Name" /> property.</param>
    /// <param name="enabled">The filter value for the <see cref="Artist.Enabled" /> property.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all artists corresponding to the request configuration.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ArtistPageResponse>> GetPagedArtistsAsync([Required][FromQuery] int pageSize, [Required][FromQuery] int pageIndex, [FromQuery] string? name, [FromQuery] bool? enabled)
    {
        var artistRequest = new ArtistRequest()
        {
            PageSize = pageSize,
            PageIndex = pageIndex,
            Name = name,
            Enabled = enabled,
        };

        ArtistPageResponse pageResponse = await m_artistService.GetArtistsAsync(artistRequest);
        return Ok(pageResponse);
    }

    /// <summary>
    /// Asynchronously gets all artist relationships by an artist's unique identifier.
    /// </summary>
    /// <param name="artistId">The artist's unique identifier.</param>
    /// <param name="includeReverseRelationships">A boolean value specifying whether reverse relationships should be included.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all artist relationships.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ArtistRelationship[]>> GetArtistRelationshipsAsync([Required][FromQuery] Guid artistId, [FromQuery] bool includeReverseRelationships)
    {
        ArtistRelationship[] artistRelationships = await m_artistService.GetArtistRelationshipsAsync(artistId, includeReverseRelationships);
        return Ok(artistRelationships);
    }

    /// <summary>
    /// Asynchronously creates a new artist.
    /// </summary>
    /// <param name="artist">The artist to create.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created artist with properties like <see cref="Artist.Id" /> set.
    /// </returns>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Artist>> CreateArtistAsync([Required][FromBody] Artist artist)
    {
        Artist createdArtist = await m_artistService.CreateArtistAsync(artist);
        return Ok(createdArtist);
    }

    /// <summary>
    /// Asynchronously updates an existing artist.
    /// </summary>
    /// <param name="artist">The artist to update.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateArtistAsync([Required][FromBody] Artist artist)
    {
        var result = await m_artistService.UpdateArtistAsync(artist);
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Asynchronously deletes an existing artist.
    /// </summary>
    /// <param name="artistId">The unique identifier of the artist to delete.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteArtistAsync([Required][FromQuery] Guid artistId)
    {
        var result = await m_artistService.DeleteArtistAsync(artistId);
        return result ? Ok() : NotFound();
    }
}
