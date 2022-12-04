using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Data.Repositories;
using MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Repositories;

/// <summary>
/// Represents a SQL Server - specific implementation of the artist repository.
/// </summary>
public class SqlServerArtistRepository : IArtistRepository
{
    /// <inheritdoc />
    public Task<ArtistDto?> GetArtistAsync(Guid artistId) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ArtistDto[]> GetArtistsAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ArtistDto[]> GetArtistsAsync(IEnumerable<Guid> artistIds) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ArtistDto[]> GetArtistsAsync(EntityCollectionProcessor<ArtistDto> collectionProcessor) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<PageResponseDto<ArtistDto>> GetArtistsAsync(ArtistRequestDto artistRequest) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ArtistDto> CreateArtistAsync(ArtistDto artist) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> UpdateArtistAsync(ArtistDto artist) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> DeleteArtistAsync(Guid artistId) => throw new NotImplementedException();
}
