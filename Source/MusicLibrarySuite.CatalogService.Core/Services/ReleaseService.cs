using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services;

/// <summary>
/// Represents a release service.
/// </summary>
public class ReleaseService : IReleaseService
{
    /// <inheritdoc />
    public Task<Release?> GetReleaseAsync(Guid releaseId) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Release[]> GetReleasesAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Release[]> GetReleasesAsync(IEnumerable<Guid> releaseIds) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleasePageResponse> GetReleasesAsync(ReleaseRequest releaseRequest) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Release> CreateReleaseAsync(Release release) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> UpdateReleaseAsync(Release release) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> DeleteReleaseAsync(Guid releaseId) => throw new NotImplementedException();
}
