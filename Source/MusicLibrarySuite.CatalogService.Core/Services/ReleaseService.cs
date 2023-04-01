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
        return release;
    }

    /// <inheritdoc />
    public async Task<Release[]> GetReleasesAsync()
    {
        ReleaseDto[] releaseDtoArray = await m_releaseRepository.GetReleasesAsync();
        Release[] releaseArray = m_mapper.Map<Release[]>(releaseDtoArray);
        return releaseArray;
    }

    /// <inheritdoc />
    public async Task<Release[]> GetReleasesAsync(IEnumerable<Guid> releaseIds)
    {
        ReleaseDto[] releaseDtoArray = await m_releaseRepository.GetReleasesAsync(releaseIds);
        Release[] releaseArray = m_mapper.Map<Release[]>(releaseDtoArray);
        return releaseArray;
    }

    /// <inheritdoc />
    public async Task<ReleasePageResponse> GetReleasesAsync(ReleasePageRequest releasePageRequest)
    {
        ReleasePageRequestDto releasePageRequestDto = m_mapper.Map<ReleasePageRequestDto>(releasePageRequest);
        PageResponseDto<ReleaseDto> releasePageResponseDto = await m_releaseRepository.GetReleasesAsync(releasePageRequestDto);
        ReleasePageResponse releasePageResponse = m_mapper.Map<ReleasePageResponse>(releasePageResponseDto);
        return releasePageResponse;
    }

    /// <inheritdoc />
    public async Task<ReleaseRelationship[]> GetReleaseRelationshipsAsync(Guid releaseId, bool includeReverseRelationships = false)
    {
        ReleaseRelationshipDto[] releaseRelationshipDtoArray = await m_releaseRepository.GetReleaseRelationshipsAsync(releaseId, includeReverseRelationships);
        ReleaseRelationship[] releaseRelationshipArray = m_mapper.Map<ReleaseRelationship[]>(releaseRelationshipDtoArray);
        return releaseRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<ReleaseToProductRelationship[]> GetReleaseToProductRelationshipsAsync(Guid releaseId)
    {
        ReleaseToProductRelationshipDto[] releaseToProductRelationshipDtoArray = await m_releaseRepository.GetReleaseToProductRelationshipsAsync(releaseId);
        ReleaseToProductRelationship[] releaseToProductRelationshipArray = m_mapper.Map<ReleaseToProductRelationship[]>(releaseToProductRelationshipDtoArray);
        return releaseToProductRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<ReleaseToProductRelationship[]> GetReleaseToProductRelationshipsByProductAsync(Guid productId)
    {
        ReleaseToProductRelationshipDto[] releaseToProductRelationshipDtoArray = await m_releaseRepository.GetReleaseToProductRelationshipsByProductAsync(productId);
        ReleaseToProductRelationship[] releaseToProductRelationshipArray = m_mapper.Map<ReleaseToProductRelationship[]>(releaseToProductRelationshipDtoArray);
        return releaseToProductRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<ReleaseToReleaseGroupRelationship[]> GetReleaseToReleaseGroupRelationshipsAsync(Guid releaseId)
    {
        ReleaseToReleaseGroupRelationshipDto[] releaseToReleaseGroupRelationshipDtoArray = await m_releaseRepository.GetReleaseToReleaseGroupRelationshipsAsync(releaseId);
        ReleaseToReleaseGroupRelationship[] releaseToReleaseGroupRelationshipArray = m_mapper.Map<ReleaseToReleaseGroupRelationship[]>(releaseToReleaseGroupRelationshipDtoArray);
        return releaseToReleaseGroupRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<ReleaseToReleaseGroupRelationship[]> GetReleaseToReleaseGroupRelationshipsByReleaseGroupAsync(Guid releaseGroupId)
    {
        ReleaseToReleaseGroupRelationshipDto[] releaseToReleaseGroupRelationshipDtoArray = await m_releaseRepository.GetReleaseToReleaseGroupRelationshipsByReleaseGroupAsync(releaseGroupId);
        ReleaseToReleaseGroupRelationship[] releaseToReleaseGroupRelationshipArray = m_mapper.Map<ReleaseToReleaseGroupRelationship[]>(releaseToReleaseGroupRelationshipDtoArray);
        return releaseToReleaseGroupRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<ReleaseMediaToProductRelationship[]> GetReleaseMediaToProductRelationshipsAsync(Guid releaseId)
    {
        ReleaseMediaToProductRelationshipDto[] releaseMediaToProductRelationshipDtoArray = await m_releaseRepository.GetReleaseMediaToProductRelationshipsAsync(releaseId);
        ReleaseMediaToProductRelationship[] releaseMediaToProductRelationshipArray = m_mapper.Map<ReleaseMediaToProductRelationship[]>(releaseMediaToProductRelationshipDtoArray);
        return releaseMediaToProductRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<ReleaseMediaToProductRelationship[]> GetReleaseMediaToProductRelationshipsByProductAsync(Guid productId)
    {
        ReleaseMediaToProductRelationshipDto[] releaseMediaToProductRelationshipDtoArray = await m_releaseRepository.GetReleaseMediaToProductRelationshipsByProductAsync(productId);
        ReleaseMediaToProductRelationship[] releaseMediaToProductRelationshipArray = m_mapper.Map<ReleaseMediaToProductRelationship[]>(releaseMediaToProductRelationshipDtoArray);
        return releaseMediaToProductRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<ReleaseTrackToProductRelationship[]> GetReleaseTrackToProductRelationshipsAsync(Guid releaseId)
    {
        ReleaseTrackToProductRelationshipDto[] releaseTrackToProductRelationshipDtoArray = await m_releaseRepository.GetReleaseTrackToProductRelationshipsAsync(releaseId);
        ReleaseTrackToProductRelationship[] releaseTrackToProductRelationshipArray = m_mapper.Map<ReleaseTrackToProductRelationship[]>(releaseTrackToProductRelationshipDtoArray);
        return releaseTrackToProductRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<ReleaseTrackToProductRelationship[]> GetReleaseTrackToProductRelationshipsByProductAsync(Guid productId)
    {
        ReleaseTrackToProductRelationshipDto[] releaseTrackToProductRelationshipDtoArray = await m_releaseRepository.GetReleaseTrackToProductRelationshipsByProductAsync(productId);
        ReleaseTrackToProductRelationship[] releaseTrackToProductRelationshipArray = m_mapper.Map<ReleaseTrackToProductRelationship[]>(releaseTrackToProductRelationshipDtoArray);
        return releaseTrackToProductRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<ReleaseTrackToWorkRelationship[]> GetReleaseTrackToWorkRelationshipsAsync(Guid releaseId)
    {
        ReleaseTrackToWorkRelationshipDto[] releaseTrackToWorkRelationshipDtoArray = await m_releaseRepository.GetReleaseTrackToWorkRelationshipsAsync(releaseId);
        ReleaseTrackToWorkRelationship[] releaseTrackToWorkRelationshipArray = m_mapper.Map<ReleaseTrackToWorkRelationship[]>(releaseTrackToWorkRelationshipDtoArray);
        return releaseTrackToWorkRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<ReleaseTrackToWorkRelationship[]> GetReleaseTrackToWorkRelationshipsByWorkAsync(Guid workId)
    {
        ReleaseTrackToWorkRelationshipDto[] releaseTrackToWorkRelationshipDtoArray = await m_releaseRepository.GetReleaseTrackToWorkRelationshipsByWorkAsync(workId);
        ReleaseTrackToWorkRelationship[] releaseTrackToWorkRelationshipArray = m_mapper.Map<ReleaseTrackToWorkRelationship[]>(releaseTrackToWorkRelationshipDtoArray);
        return releaseTrackToWorkRelationshipArray;
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
    public async Task<bool> UpdateReleaseToProductRelationshipsOrderAsync(ReleaseToProductRelationship[] releaseToProductRelationships, bool useReferenceOrder)
    {
        ReleaseToProductRelationshipDto[] releaseToProductRelationshipDtoArray = m_mapper.Map<ReleaseToProductRelationshipDto[]>(releaseToProductRelationships);
        var updated = await m_releaseRepository.UpdateReleaseToProductRelationshipsOrderAsync(releaseToProductRelationshipDtoArray, useReferenceOrder);
        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseToReleaseGroupRelationshipsOrderAsync(ReleaseToReleaseGroupRelationship[] releaseToReleaseGroupRelationships, bool useReferenceOrder)
    {
        ReleaseToReleaseGroupRelationshipDto[] releaseToReleaseGroupRelationshipDtoArray = m_mapper.Map<ReleaseToReleaseGroupRelationshipDto[]>(releaseToReleaseGroupRelationships);
        var updated = await m_releaseRepository.UpdateReleaseToReleaseGroupRelationshipsOrderAsync(releaseToReleaseGroupRelationshipDtoArray, useReferenceOrder);
        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseMediaToProductRelationshipsOrderAsync(ReleaseMediaToProductRelationship[] releaseMediaToProductRelationships, bool useReferenceOrder)
    {
        ReleaseMediaToProductRelationshipDto[] releaseMediaToProductRelationshipDtoArray = m_mapper.Map<ReleaseMediaToProductRelationshipDto[]>(releaseMediaToProductRelationships);
        var updated = await m_releaseRepository.UpdateReleaseMediaToProductRelationshipsOrderAsync(releaseMediaToProductRelationshipDtoArray, useReferenceOrder);
        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseTrackToProductRelationshipsOrderAsync(ReleaseTrackToProductRelationship[] releaseTrackToProductRelationships, bool useReferenceOrder)
    {
        ReleaseTrackToProductRelationshipDto[] releaseTrackToProductRelationshipDtoArray = m_mapper.Map<ReleaseTrackToProductRelationshipDto[]>(releaseTrackToProductRelationships);
        var updated = await m_releaseRepository.UpdateReleaseTrackToProductRelationshipsOrderAsync(releaseTrackToProductRelationshipDtoArray, useReferenceOrder);
        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseTrackToWorkRelationshipsOrderAsync(ReleaseTrackToWorkRelationship[] releaseTrackToWorkRelationships, bool useReferenceOrder)
    {
        ReleaseTrackToWorkRelationshipDto[] releaseTrackToWorkRelationshipDtoArray = m_mapper.Map<ReleaseTrackToWorkRelationshipDto[]>(releaseTrackToWorkRelationships);
        var updated = await m_releaseRepository.UpdateReleaseTrackToWorkRelationshipsOrderAsync(releaseTrackToWorkRelationshipDtoArray, useReferenceOrder);
        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteReleaseAsync(Guid releaseId)
    {
        var deleted = await m_releaseRepository.DeleteReleaseAsync(releaseId);
        return deleted;
    }
}
