using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services.Abstractions;

/// <summary>
/// Defines a set of members a genre service should implement.
/// </summary>
public interface IGenreService
{
    /// <summary>
    /// Asynchronously gets a genre by its unique identifier.
    /// </summary>
    /// <param name="genreId">The genre's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the genre found or <see langword="null" />.
    /// </returns>
    public Task<Genre?> GetGenreAsync(Guid genreId);

    /// <summary>
    /// Asynchronously gets all genres.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all genres.
    /// </returns>
    public Task<Genre[]> GetGenresAsync();

    /// <summary>
    /// Asynchronously gets genres by a collection of unique identifiers.
    /// </summary>
    /// <param name="genreIds">The collection of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found genres.
    /// </returns>
    public Task<Genre[]> GetGenresAsync(IEnumerable<Guid> genreIds);

    /// <summary>
    /// Asynchronously gets genres by a genre page request.
    /// </summary>
    /// <param name="genreRequest">The genre page request configuration.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all genres corresponding to the request configuration.
    /// </returns>
    public Task<GenrePageResponse> GetGenresAsync(GenreRequest genreRequest);

    /// <summary>
    /// Asynchronously gets all genre relationships by a genre's unique identifier.
    /// </summary>
    /// <param name="genreId">The genre's unique identifier.</param>
    /// <param name="includeReverseRelationships">A boolean value specifying whether reverse relationships should be included.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all genre relationships.
    /// </returns>
    public Task<GenreRelationship[]> GetGenreRelationshipsAsync(Guid genreId, bool includeReverseRelationships = false);

    /// <summary>
    /// Asynchronously creates a new genre.
    /// </summary>
    /// <param name="genre">The genre to create.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created genre with properties like <see cref="Genre.Id" /> set.
    /// </returns>
    public Task<Genre> CreateGenreAsync(Genre genre);

    /// <summary>
    /// Asynchronously updates an existing genre.
    /// </summary>
    /// <param name="genre">The genre to update.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the genre was found and updated.
    /// </returns>
    public Task<bool> UpdateGenreAsync(Genre genre);

    /// <summary>
    /// Asynchronously deletes an existing genre.
    /// </summary>
    /// <param name="genreId">The unique identifier of the genre to delete.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the genre was found and deleted.
    /// </returns>
    public Task<bool> DeleteGenreAsync(Guid genreId);
}
