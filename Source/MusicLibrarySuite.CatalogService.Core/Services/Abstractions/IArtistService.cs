using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services.Abstractions;

/// <summary>
/// Defines a set of members an artist service should implement.
/// </summary>
public interface IArtistService
{
    /// <summary>
    /// Asynchronously gets an artist by its unique identifier.
    /// </summary>
    /// <param name="artistId">The artist's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the artist found or <see langword="null" />.
    /// </returns>
    public Task<Artist?> GetArtistAsync(Guid artistId);

    /// <summary>
    /// Asynchronously gets all artists.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all artists.
    /// </returns>
    public Task<Artist[]> GetArtistsAsync();

    /// <summary>
    /// Asynchronously gets artists by a collection of unique identifiers.
    /// </summary>
    /// <param name="artistIds">The collection of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found artists.
    /// </returns>
    public Task<Artist[]> GetArtistsAsync(IEnumerable<Guid> artistIds);

    /// <summary>
    /// Asynchronously gets artists by an artist page request.
    /// </summary>
    /// <param name="artistRequest">The artist page request configuration.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all artists corresponding to the request configuration.
    /// </returns>
    public Task<ArtistPageResponse> GetArtistsAsync(ArtistRequest artistRequest);

    /// <summary>
    /// Asynchronously creates a new artist.
    /// </summary>
    /// <param name="artist">The artist to create.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created artist with properties like <see cref="Artist.Id" /> set.
    /// </returns>
    public Task<Artist> CreateArtistAsync(Artist artist);

    /// <summary>
    /// Asynchronously updates an existing artist.
    /// </summary>
    /// <param name="artist">The artist to update.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the artist was found and updated.
    /// </returns>
    public Task<bool> UpdateArtistAsync(Artist artist);

    /// <summary>
    /// Asynchronously deletes an existing artist.
    /// </summary>
    /// <param name="artistId">The unique identifier of the artist to delete.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the artist was found and deleted.
    /// </returns>
    public Task<bool> DeleteArtistAsync(Guid artistId);
}
