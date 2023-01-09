using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Data.Repositories;
using MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Repositories;

/// <summary>
/// Represents a SQL Server - specific implementation of the release repository.
/// </summary>
public class SqlServerReleaseRepository : IReleaseRepository
{
    /// <inheritdoc />
    public Task<ReleaseDto?> GetReleaseAsync(Guid releaseId) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleaseDto[]> GetReleasesAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleaseDto[]> GetReleasesAsync(IEnumerable<Guid> releaseIds) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleaseDto[]> GetReleasesAsync(EntityCollectionProcessor<ReleaseDto> collectionProcessor) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<PageResponseDto<ReleaseDto>> GetReleasesAsync(ReleaseRequestDto releaseRequest) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ReleaseDto> CreateReleaseAsync(ReleaseDto release) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> UpdateReleaseAsync(ReleaseDto release) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> DeleteReleaseAsync(Guid releaseId) => throw new NotImplementedException();
}
