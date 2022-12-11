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
/// Represents an artist service.
/// </summary>
public class ArtistService : IArtistService
{
    private readonly IArtistRepository m_artistRepository;
    private readonly IMapper m_mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistService" /> type using the specified services.
    /// </summary>
    /// <param name="artistRepository">The artist repository.</param>
    /// <param name="mapper">The AutoMapper mapper.</param>
    public ArtistService(IArtistRepository artistRepository, IMapper mapper)
    {
        m_artistRepository = artistRepository;
        m_mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Artist?> GetArtistAsync(Guid artistId)
    {
        ArtistDto? artistDto = await m_artistRepository.GetArtistAsync(artistId);
        Artist? artist = m_mapper.Map<Artist?>(artistDto);
        return artist;
    }

    /// <inheritdoc />
    public async Task<Artist[]> GetArtistsAsync()
    {
        ArtistDto[] artistDtoArray = await m_artistRepository.GetArtistsAsync();
        Artist[] artistArray = m_mapper.Map<Artist[]>(artistDtoArray);
        return artistArray;
    }

    /// <inheritdoc />
    public async Task<Artist[]> GetArtistsAsync(IEnumerable<Guid> artistIds)
    {
        ArtistDto[] artistDtoArray = await m_artistRepository.GetArtistsAsync(artistIds);
        Artist[] artistArray = m_mapper.Map<Artist[]>(artistDtoArray);
        return artistArray;
    }

    /// <inheritdoc />
    public async Task<ArtistPageResponse> GetArtistsAsync(ArtistRequest artistRequest)
    {
        ArtistRequestDto artistRequestDto = m_mapper.Map<ArtistRequestDto>(artistRequest);
        PageResponseDto<ArtistDto> pageResponseDto = await m_artistRepository.GetArtistsAsync(artistRequestDto);
        ArtistPageResponse pageResponse = m_mapper.Map<ArtistPageResponse>(pageResponseDto);

        pageResponse.CompletedOn = DateTimeOffset.Now;

        return pageResponse;
    }

    /// <inheritdoc />
    public async Task<Artist> CreateArtistAsync(Artist artist)
    {
        ArtistDto artistDto = m_mapper.Map<ArtistDto>(artist);
        ArtistDto createdArtistDto = await m_artistRepository.CreateArtistAsync(artistDto);
        Artist createdArtist = m_mapper.Map<Artist>(createdArtistDto);
        return createdArtist;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateArtistAsync(Artist artist)
    {
        ArtistDto artistDto = m_mapper.Map<ArtistDto>(artist);
        var updated = await m_artistRepository.UpdateArtistAsync(artistDto);
        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteArtistAsync(Guid artistId)
    {
        var deleted = await m_artistRepository.DeleteArtistAsync(artistId);
        return deleted;
    }
}
