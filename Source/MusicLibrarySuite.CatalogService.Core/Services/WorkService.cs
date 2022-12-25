using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services;

/// <summary>
/// Represents a work service.
/// </summary>
public class WorkService : IWorkService
{
    /// <inheritdoc />
    public Task<Work?> GetWorkAsync(Guid workId) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Work[]> GetWorksAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Work[]> GetWorksAsync(IEnumerable<Guid> workIds) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<WorkPageResponse> GetWorksAsync(WorkRequest workRequest) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Work> CreateWorkAsync(Work work) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> UpdateWorkAsync(Work work) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> DeleteWorkAsync(Guid workId) => throw new NotImplementedException();
}
