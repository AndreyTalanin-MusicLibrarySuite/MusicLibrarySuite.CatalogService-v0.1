using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services;

/// <summary>
/// Represents a release group service.
/// </summary>
public class ReleaseGroupService : IReleaseGroupService
{
    /// <inheritdoc />
    public Task<ReleaseGroup?> GetReleaseGroupAsync(Guid releaseGroupId) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleaseGroup[]> GetReleaseGroupsAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleaseGroup[]> GetReleaseGroupsAsync(IEnumerable<Guid> releaseGroupIds) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleaseGroupPageResponse> GetReleaseGroupsAsync(ReleaseGroupRequest releaseGroupRequest) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleaseGroup> CreateReleaseGroupAsync(ReleaseGroup releaseGroup) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> UpdateReleaseGroupAsync(ReleaseGroup releaseGroup) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> DeleteReleaseGroupAsync(Guid releaseGroupId) => throw new NotImplementedException();
}
