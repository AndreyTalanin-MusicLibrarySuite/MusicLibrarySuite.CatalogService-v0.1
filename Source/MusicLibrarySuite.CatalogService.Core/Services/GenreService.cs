using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services;

/// <summary>
/// Represents a genre service.
/// </summary>
public class GenreService : IGenreService
{
    /// <inheritdoc />
    public Task<Genre?> GetGenreAsync(Guid genreId) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Genre[]> GetGenresAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Genre[]> GetGenresAsync(IEnumerable<Guid> genreIds) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<GenrePageResponse> GetGenresAsync(GenreRequest genreRequest) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Genre> CreateGenreAsync(Genre genre) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> UpdateGenreAsync(Genre genre) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> DeleteGenreAsync(Guid genreId) => throw new NotImplementedException();
}
