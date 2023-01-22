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
/// Represents a release service.
/// </summary>
public class ReleaseService : IReleaseService
{
    private readonly IReleaseRepository m_releaseRepository;
    private readonly IMapper m_mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReleaseService" /> type using the specified services.
    /// </summary>
    /// <param name="releaseRepository">The release repository.</param>
    /// <param name="mapper">The AutoMapper mapper.</param>
    public ReleaseService(IReleaseRepository releaseRepository, IMapper mapper)
    {
        m_releaseRepository = releaseRepository;
        m_mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Release?> GetReleaseAsync(Guid releaseId)
    {
        ReleaseDto? releaseDto = await m_releaseRepository.GetReleaseAsync(releaseId);
        Release? release = m_mapper.Map<Release?>(releaseDto);

        if (release is not null)
        {
            SetReleaseMediaDisplayProperties(release);
            foreach (ReleaseMedia releaseMedia in release.ReleaseMediaCollection)
            {
                SetReleaseTrackDisplayProperties(releaseMedia);
            }
        }

        return release;
    }

    /// <inheritdoc />
    public async Task<Release[]> GetReleasesAsync()
    {
        ReleaseDto[] releaseDtoArray = await m_releaseRepository.GetReleasesAsync();
        Release[] releaseArray = m_mapper.Map<Release[]>(releaseDtoArray);

        foreach (Release release in releaseArray)
        {
            SetReleaseMediaDisplayProperties(release);
            foreach (ReleaseMedia releaseMedia in release.ReleaseMediaCollection)
            {
                SetReleaseTrackDisplayProperties(releaseMedia);
            }
        }

        return releaseArray;
    }

    /// <inheritdoc />
    public async Task<Release[]> GetReleasesAsync(IEnumerable<Guid> releaseIds)
    {
        ReleaseDto[] releaseDtoArray = await m_releaseRepository.GetReleasesAsync(releaseIds);
        Release[] releaseArray = m_mapper.Map<Release[]>(releaseDtoArray);

        foreach (Release release in releaseArray)
        {
            SetReleaseMediaDisplayProperties(release);
            foreach (ReleaseMedia releaseMedia in release.ReleaseMediaCollection)
            {
                SetReleaseTrackDisplayProperties(releaseMedia);
            }
        }

        return releaseArray;
    }

    /// <inheritdoc />
    public async Task<ReleasePageResponse> GetReleasesAsync(ReleaseRequest releaseRequest)
    {
        ReleaseRequestDto releaseRequestDto = m_mapper.Map<ReleaseRequestDto>(releaseRequest);
        PageResponseDto<ReleaseDto> pageResponseDto = await m_releaseRepository.GetReleasesAsync(releaseRequestDto);
        ReleasePageResponse pageResponse = m_mapper.Map<ReleasePageResponse>(pageResponseDto);

        foreach (Release release in pageResponse.Items)
        {
            SetReleaseMediaDisplayProperties(release);
            foreach (ReleaseMedia releaseMedia in release.ReleaseMediaCollection)
            {
                SetReleaseTrackDisplayProperties(releaseMedia);
            }
        }

        pageResponse.CompletedOn = DateTimeOffset.Now;

        return pageResponse;
    }

    /// <inheritdoc />
    public async Task<ReleaseRelationship[]> GetReleaseRelationshipsAsync(Guid releaseId, bool includeReverseRelationships = false)
    {
        ReleaseRelationshipDto[] releaseRelationshipDtoArray = await m_releaseRepository.GetReleaseRelationshipsAsync(releaseId, includeReverseRelationships);
        ReleaseRelationship[] releaseRelationshipArray = m_mapper.Map<ReleaseRelationship[]>(releaseRelationshipDtoArray);
        return releaseRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<Release> CreateReleaseAsync(Release release)
    {
        ReleaseDto releaseDto = m_mapper.Map<ReleaseDto>(release);
        ReleaseDto createdReleaseDto = await m_releaseRepository.CreateReleaseAsync(releaseDto);
        Release createdRelease = m_mapper.Map<Release>(createdReleaseDto);
        return createdRelease;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseAsync(Release release)
    {
        ReleaseDto releaseDto = m_mapper.Map<ReleaseDto>(release);
        var updated = await m_releaseRepository.UpdateReleaseAsync(releaseDto);
        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteReleaseAsync(Guid releaseId)
    {
        var deleted = await m_releaseRepository.DeleteReleaseAsync(releaseId);
        return deleted;
    }

    private static void SetReleaseMediaDisplayProperties(Release release)
    {
        foreach (ReleaseMedia releaseMedia in release.ReleaseMediaCollection)
        {
            releaseMedia.TotalMediaCount = (short)release.ReleaseMediaCollection.Count;
        }
    }

    private static void SetReleaseTrackDisplayProperties(ReleaseMedia releaseMedia)
    {
        foreach (ReleaseTrack releaseTrack in releaseMedia.ReleaseTrackCollection)
        {
            releaseTrack.TotalTrackCount = (short)releaseMedia.ReleaseTrackCollection.Count;
            releaseTrack.TotalMediaCount = releaseMedia.TotalMediaCount;
        }
    }
}
