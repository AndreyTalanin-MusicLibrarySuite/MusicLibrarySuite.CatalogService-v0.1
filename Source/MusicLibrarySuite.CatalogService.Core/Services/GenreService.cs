using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services;

/// <summary>
/// Represents a genre service.
/// </summary>
public class GenreService : IGenreService
{
    private readonly IGenreRepository m_genreRepository;
    private readonly IMapper m_mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenreService" /> type using the specified services.
    /// </summary>
    /// <param name="genreRepository">The genre repository.</param>
    /// <param name="mapper">The AutoMapper mapper.</param>
    public GenreService(IGenreRepository genreRepository, IMapper mapper)
    {
        m_genreRepository = genreRepository;
        m_mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Genre?> GetGenreAsync(Guid genreId)
    {
        GenreDto? genreDto = await m_genreRepository.GetGenreAsync(genreId);
        Genre? genre = m_mapper.Map<Genre?>(genreDto);
        return genre;
    }

    /// <inheritdoc />
    public async Task<Genre[]> GetGenresAsync()
    {
        GenreDto[] genreDtoArray = await m_genreRepository.GetGenresAsync();
        Genre[] genreArray = m_mapper.Map<Genre[]>(genreDtoArray);
        return genreArray;
    }

    /// <inheritdoc />
    public async Task<Genre[]> GetGenresAsync(IEnumerable<Guid> genreIds)
    {
        GenreDto[] genreDtoArray = await m_genreRepository.GetGenresAsync(genreIds);
        Genre[] genreArray = m_mapper.Map<Genre[]>(genreDtoArray);
        return genreArray;
    }

    /// <inheritdoc/>
    public async Task<GenrePageResponse> GetGenresAsync(GenrePageRequest genrePageRequest)
    {
        GenrePageRequestDto genrePageRequestDto = m_mapper.Map<GenrePageRequestDto>(genrePageRequest);
        PageResponseDto<GenreDto> genrePageResponseDto = await m_genreRepository.GetGenresAsync(genrePageRequestDto);
        GenrePageResponse genrePageResponse = m_mapper.Map<GenrePageResponse>(genrePageResponseDto);
        return genrePageResponse;
    }

    /// <inheritdoc />
    public async Task<GenreRelationship[]> GetGenreRelationshipsAsync(Guid genreId, bool includeReverseRelationships = false)
    {
        GenreRelationshipDto[] genreRelationshipDtoArray = await m_genreRepository.GetGenreRelationshipsAsync(genreId, includeReverseRelationships);
        GenreRelationship[] genreRelationshipArray = m_mapper.Map<GenreRelationship[]>(genreRelationshipDtoArray);
        return genreRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<Genre> CreateGenreAsync(Genre genre)
    {
        GenreDto genreDto = m_mapper.Map<GenreDto>(genre);
        GenreDto createdGenreDto = await m_genreRepository.CreateGenreAsync(genreDto);
        Genre createdGenre = m_mapper.Map<Genre>(createdGenreDto);
        return createdGenre;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateGenreAsync(Genre genre)
    {
        GenreDto genreDto = m_mapper.Map<GenreDto>(genre);
        var updated = await m_genreRepository.UpdateGenreAsync(genreDto);
        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteGenreAsync(Guid genreId)
    {
        var deleted = await m_genreRepository.DeleteGenreAsync(genreId);
        return deleted;
    }
}
