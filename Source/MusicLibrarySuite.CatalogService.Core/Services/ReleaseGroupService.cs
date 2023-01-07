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
/// Represents a release group service.
/// </summary>
public class ReleaseGroupService : IReleaseGroupService
{
    private readonly IReleaseGroupRepository m_releaseGroupRepository;
    private readonly IMapper m_mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReleaseGroupService" /> type using the specified services.
    /// </summary>
    /// <param name="releaseGroupRepository">The release group repository.</param>
    /// <param name="mapper">The AutoMapper mapper.</param>
    public ReleaseGroupService(IReleaseGroupRepository releaseGroupRepository, IMapper mapper)
    {
        m_releaseGroupRepository = releaseGroupRepository;
        m_mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<ReleaseGroup?> GetReleaseGroupAsync(Guid releaseGroupId)
    {
        ReleaseGroupDto? releaseGroupDto = await m_releaseGroupRepository.GetReleaseGroupAsync(releaseGroupId);
        ReleaseGroup? releaseGroup = m_mapper.Map<ReleaseGroup?>(releaseGroupDto);
        return releaseGroup;
    }

    /// <inheritdoc />
    public async Task<ReleaseGroup[]> GetReleaseGroupsAsync()
    {
        ReleaseGroupDto[] releaseGroupDtoArray = await m_releaseGroupRepository.GetReleaseGroupsAsync();
        ReleaseGroup[] releaseGroupArray = m_mapper.Map<ReleaseGroup[]>(releaseGroupDtoArray);
        return releaseGroupArray;
    }

    /// <inheritdoc />
    public async Task<ReleaseGroup[]> GetReleaseGroupsAsync(IEnumerable<Guid> releaseGroupIds)
    {
        ReleaseGroupDto[] releaseGroupDtoArray = await m_releaseGroupRepository.GetReleaseGroupsAsync(releaseGroupIds);
        ReleaseGroup[] releaseGroupArray = m_mapper.Map<ReleaseGroup[]>(releaseGroupDtoArray);
        return releaseGroupArray;
    }

    /// <inheritdoc />
    public async Task<ReleaseGroupPageResponse> GetReleaseGroupsAsync(ReleaseGroupRequest releaseGroupRequest)
    {
        ReleaseGroupRequestDto releaseGroupRequestDto = m_mapper.Map<ReleaseGroupRequestDto>(releaseGroupRequest);
        PageResponseDto<ReleaseGroupDto> pageResponseDto = await m_releaseGroupRepository.GetReleaseGroupsAsync(releaseGroupRequestDto);
        ReleaseGroupPageResponse pageResponse = m_mapper.Map<ReleaseGroupPageResponse>(pageResponseDto);

        pageResponse.CompletedOn = DateTimeOffset.Now;

        return pageResponse;
    }

    /// <inheritdoc />
    public async Task<ReleaseGroup> CreateReleaseGroupAsync(ReleaseGroup releaseGroup)
    {
        ReleaseGroupDto releaseGroupDto = m_mapper.Map<ReleaseGroupDto>(releaseGroup);
        ReleaseGroupDto createdReleaseGroupDto = await m_releaseGroupRepository.CreateReleaseGroupAsync(releaseGroupDto);
        ReleaseGroup createdReleaseGroup = m_mapper.Map<ReleaseGroup>(createdReleaseGroupDto);
        return createdReleaseGroup;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseGroupAsync(ReleaseGroup releaseGroup)
    {
        ReleaseGroupDto releaseGroupDto = m_mapper.Map<ReleaseGroupDto>(releaseGroup);
        var updated = await m_releaseGroupRepository.UpdateReleaseGroupAsync(releaseGroupDto);
        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteReleaseGroupAsync(Guid releaseGroupId)
    {
        var deleted = await m_releaseGroupRepository.DeleteReleaseGroupAsync(releaseGroupId);
        return deleted;
    }
}
