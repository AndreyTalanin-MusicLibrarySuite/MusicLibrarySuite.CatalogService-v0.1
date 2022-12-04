using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

/// <summary>
/// Defines a set of members a provider-specific artist repository should implement.
/// </summary>
public interface IArtistRepository
{
    /// <summary>
    /// Asynchronously gets an artist by its unique identifier.
    /// </summary>
    /// <param name="artistId">The artist's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the artist found or <see langword="null" />.
    /// </returns>
    public Task<ArtistDto?> GetArtistAsync(Guid artistId);

    /// <summary>
    /// Asynchronously gets all artists.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all artists.
    /// </returns>
    public Task<ArtistDto[]> GetArtistsAsync();

    /// <summary>
    /// Asynchronously gets artists by a collection of unique identifiers.
    /// </summary>
    /// <param name="artistIds">The collection of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found artists.
    /// </returns>
    public Task<ArtistDto[]> GetArtistsAsync(IEnumerable<Guid> artistIds);

    /// <summary>
    /// Asynchronously gets artists filtered by a collection processor.
    /// </summary>
    /// <param name="collectionProcessor">The collection processor to filter artists.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found artists.
    /// </returns>
    public Task<ArtistDto[]> GetArtistsAsync(EntityCollectionProcessor<ArtistDto> collectionProcessor);

    /// <summary>
    /// Asynchronously gets artists by an artist page request.
    /// </summary>
    /// <param name="artistRequest">The artist page request configuration.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all artists corresponding to the request configuration.
    /// </returns>
    public Task<PageResponseDto<ArtistDto>> GetArtistsAsync(ArtistRequestDto artistRequest);

    /// <summary>
    /// Asynchronously creates a new artist.
    /// </summary>
    /// <param name="artist">The artist to create in the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created artist with properties like <see cref="ArtistDto.Id" /> set.
    /// </returns>
    public Task<ArtistDto> CreateArtistAsync(ArtistDto artist);

    /// <summary>
    /// Asynchronously updates an existing artist.
    /// </summary>
    /// <param name="artist">The artist to update in the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the artist was found and updated.
    /// </returns>
    public Task<bool> UpdateArtistAsync(ArtistDto artist);

    /// <summary>
    /// Asynchronously deletes an existing artist.
    /// </summary>
    /// <param name="artistId">The unique identifier of the artist to delete from the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the artist was found and deleted.
    /// </returns>
    public Task<bool> DeleteArtistAsync(Guid artistId);
}
