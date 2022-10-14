using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Data.Repositories;
using MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Repositories;

/// <summary>
/// Represents a SQL Server - specific implementation of the genre repository.
/// </summary>
public class SqlServerGenreRepository : IGenreRepository
{
    /// <inheritdoc />
    public Task<GenreDto?> GetGenreAsync(Guid genreId) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<GenreDto[]> GetGenresAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<GenreDto[]> GetGenresAsync(IEnumerable<Guid> genreIds) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<GenreDto[]> GetGenresAsync(EntityCollectionProcessor<GenreDto> collectionProcessor) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<PageResponseDto<GenreDto>> GetGenresAsync(GenreRequestDto genreRequest) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<GenreDto> CreateGenreAsync(GenreDto genre) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> UpdateGenreAsync(GenreDto genre) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> DeleteGenreAsync(Guid genreId) => throw new NotImplementedException();
}
