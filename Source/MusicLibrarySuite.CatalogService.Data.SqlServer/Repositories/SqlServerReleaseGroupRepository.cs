using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Data.Repositories;
using MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Repositories;

/// <summary>
/// Represents a SQL Server - specific implementation of the release group repository.
/// </summary>
public class SqlServerReleaseGroupRepository : IReleaseGroupRepository
{
    /// <inheritdoc />
    public Task<ReleaseGroupDto?> GetReleaseGroupAsync(Guid releaseGroupId) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleaseGroupDto[]> GetReleaseGroupsAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleaseGroupDto[]> GetReleaseGroupsAsync(IEnumerable<Guid> releaseGroupIds) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleaseGroupDto[]> GetReleaseGroupsAsync(EntityCollectionProcessor<ReleaseGroupDto> collectionProcessor) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<PageResponseDto<ReleaseGroupDto>> GetReleaseGroupsAsync(ReleaseGroupRequestDto releaseGroupRequest) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleaseGroupDto> CreateReleaseGroupAsync(ReleaseGroupDto releaseGroup) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> UpdateReleaseGroupAsync(ReleaseGroupDto releaseGroup) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> DeleteReleaseGroupAsync(Guid releaseGroupId) => throw new NotImplementedException();
}
