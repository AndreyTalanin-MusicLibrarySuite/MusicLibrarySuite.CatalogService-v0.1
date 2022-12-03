using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

/// <summary>
/// Defines a set of members a provider-specific genre repository should implement.
/// </summary>
public interface IGenreRepository
{
    /// <summary>
    /// Asynchronously gets a genre by its unique identifier.
    /// </summary>
    /// <param name="genreId">The genre's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the genre found or <see langword="null" />.
    /// </returns>
    public Task<GenreDto?> GetGenreAsync(Guid genreId);

    /// <summary>
    /// Asynchronously gets all genres.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all genres.
    /// </returns>
    public Task<GenreDto[]> GetGenresAsync();

    /// <summary>
    /// Asynchronously gets genres by a collection of unique identifiers.
    /// </summary>
    /// <param name="genreIds">The collection of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found genres.
    /// </returns>
    public Task<GenreDto[]> GetGenresAsync(IEnumerable<Guid> genreIds);

    /// <summary>
    /// Asynchronously gets genres filtered by a collection processor.
    /// </summary>
    /// <param name="collectionProcessor">The collection processor to filter genres.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found genres.
    /// </returns>
    public Task<GenreDto[]> GetGenresAsync(EntityCollectionProcessor<GenreDto> collectionProcessor);

    /// <summary>
    /// Asynchronously gets genres by a genre page request.
    /// </summary>
    /// <param name="genreRequest">The genre page request configuration.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all genres corresponding to the request configuration.
    /// </returns>
    public Task<PageResponseDto<GenreDto>> GetGenresAsync(GenreRequestDto genreRequest);

    /// <summary>
    /// Asynchronously gets all genre relationships by a genre's unique identifier.
    /// </summary>
    /// <param name="genreId">The genre's unique identifier.</param>
    /// <param name="includeReverseRelationships">A boolean value specifying whether reverse relationships should be included.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all genre relationships.
    /// </returns>
    public Task<GenreRelationshipDto[]> GetGenreRelationshipsAsync(Guid genreId, bool includeReverseRelationships = false);

    /// <summary>
    /// Asynchronously creates a new genre.
    /// </summary>
    /// <param name="genre">The genre to create in the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created genre with properties like <see cref="GenreDto.Id" /> set.
    /// </returns>
    public Task<GenreDto> CreateGenreAsync(GenreDto genre);

    /// <summary>
    /// Asynchronously updates an existing genre.
    /// </summary>
    /// <param name="genre">The genre to update in the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the genre was found and updated.
    /// </returns>
    public Task<bool> UpdateGenreAsync(GenreDto genre);

    /// <summary>
    /// Asynchronously deletes an existing genre.
    /// </summary>
    /// <param name="genreId">The unique identifier of the genre to delete from the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the genre was found and deleted.
    /// </returns>
    public Task<bool> DeleteGenreAsync(Guid genreId);
}
