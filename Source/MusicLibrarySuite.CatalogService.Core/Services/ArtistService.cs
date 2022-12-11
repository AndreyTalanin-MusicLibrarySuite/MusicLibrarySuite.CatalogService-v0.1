using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services;

/// <summary>
/// Represents an artist service.
/// </summary>
public class ArtistService : IArtistService
{
    /// <inheritdoc />
    public Task<Artist?> GetArtistAsync(Guid artistId) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Artist[]> GetArtistsAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Artist[]> GetArtistsAsync(IEnumerable<Guid> artistIds) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ArtistPageResponse> GetArtistsAsync(ArtistRequest artistRequest) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Artist> CreateArtistAsync(Artist artist) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> UpdateArtistAsync(Artist artist) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> DeleteArtistAsync(Guid artistId) => throw new NotImplementedException();
}
