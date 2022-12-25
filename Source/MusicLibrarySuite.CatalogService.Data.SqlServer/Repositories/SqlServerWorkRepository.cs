using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Data.Repositories;
using MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Repositories;

/// <summary>
/// Represents a SQL Server - specific implementation of the work repository.
/// </summary>
public class SqlServerWorkRepository : IWorkRepository
{
    /// <inheritdoc />
    public Task<WorkDto?> GetWorkAsync(Guid workId) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<WorkDto[]> GetWorksAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<WorkDto[]> GetWorksAsync(IEnumerable<Guid> workIds) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<WorkDto[]> GetWorksAsync(EntityCollectionProcessor<WorkDto> collectionProcessor) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<PageResponseDto<WorkDto>> GetWorksAsync(WorkRequestDto workRequest) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<WorkDto> CreateWorkAsync(WorkDto work) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> UpdateWorkAsync(WorkDto work) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> DeleteWorkAsync(Guid workId) => throw new NotImplementedException();
}
