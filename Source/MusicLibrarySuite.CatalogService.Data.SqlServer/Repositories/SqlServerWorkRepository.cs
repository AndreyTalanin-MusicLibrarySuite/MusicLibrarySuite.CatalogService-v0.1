using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using MusicLibrarySuite.CatalogService.Data.Contexts;
using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Data.Extensions.Specialized;
using MusicLibrarySuite.CatalogService.Data.Repositories;
using MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Repositories;

/// <summary>
/// Represents a SQL Server - specific implementation of the work repository.
/// </summary>
public class SqlServerWorkRepository : IWorkRepository
{
    private readonly IDbContextFactory<CatalogServiceDbContext> m_contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerWorkRepository" /> type using the specified services.
    /// </summary>
    /// <param name="contextFactory">The database context factory.</param>
    public SqlServerWorkRepository(IDbContextFactory<CatalogServiceDbContext> contextFactory)
    {
        m_contextFactory = contextFactory;
    }

    /// <inheritdoc />
    public async Task<WorkDto?> GetWorkAsync(Guid workId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var workIdParameter = new SqlParameter("WorkId", workId.AsDbValue());

        var query = $"SELECT * FROM [dbo].[ufn_GetWork] (@{workIdParameter.ParameterName})";

        WorkDto? work = await context.Works.FromSqlRaw(query, workIdParameter).AsNoTracking()
            .FirstOrDefaultAsync();

        return work;
    }

    /// <inheritdoc />
    public async Task<WorkDto[]> GetWorksAsync()
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        WorkDto[] works = await context.Works.AsNoTracking()
            .ToArrayAsync();

        return works;
    }

    /// <inheritdoc />
    public async Task<WorkDto[]> GetWorksAsync(IEnumerable<Guid> workIds)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        using var workIdsDataTable = new DataTable();
        workIdsDataTable.Columns.Add("Value", typeof(Guid));
        foreach (Guid workId in workIds)
            workIdsDataTable.Rows.Add(workId.AsDbValue());

        var workIdsParameter = new SqlParameter("WorkIds", SqlDbType.Structured) { TypeName = "[dbo].[GuidArray]", Value = workIdsDataTable };

        var query = $"SELECT * FROM [dbo].[ufn_GetWorks] (@{workIdsParameter.ParameterName})";

        WorkDto[] works = await context.Works.FromSqlRaw(query, workIdsParameter).AsNoTracking()
            .ToArrayAsync();

        return works;
    }

    /// <inheritdoc />
    public async Task<WorkDto[]> GetWorksAsync(EntityCollectionProcessor<WorkDto> collectionProcessor)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        WorkDto[] works = await collectionProcessor(context.Works.AsNoTracking())
            .ToArrayAsync();

        return works;
    }

    /// <inheritdoc />
    public async Task<PageResponseDto<WorkDto>> GetWorksAsync(WorkRequestDto workRequest)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<WorkDto> baseCollection = context.Works.AsNoTracking();

        if (workRequest.Title is not null)
            baseCollection = baseCollection.Where(work => work.Title.Contains(workRequest.Title));

        if (workRequest.Enabled is not null)
            baseCollection = baseCollection.Where(work => work.Enabled == (bool)workRequest.Enabled);

        var totalCount = await baseCollection.CountAsync();
        List<WorkDto> works = await baseCollection
            .OrderByDescending(work => work.SystemEntity)
            .ThenBy(work => work.Title)
            .Skip(workRequest.PageSize * workRequest.PageIndex)
            .Take(workRequest.PageSize)
            .ToListAsync();

        return new PageResponseDto<WorkDto>()
        {
            PageSize = workRequest.PageSize,
            PageIndex = workRequest.PageIndex,
            TotalCount = totalCount,
            Items = works,
        };
    }

    /// <inheritdoc />
    public async Task<WorkDto> CreateWorkAsync(WorkDto work)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultIdParameter;
        SqlParameter resultCreatedOnParameter;
        SqlParameter resultUpdatedOnParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(WorkDto.Id), work.Id.AsDbValue()),
            new SqlParameter(nameof(WorkDto.Title), work.Title.AsDbValue()),
            new SqlParameter(nameof(WorkDto.Description), work.Description.AsDbValue()),
            new SqlParameter(nameof(WorkDto.DisambiguationText), work.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(WorkDto.InternationalStandardMusicalWorkCode), work.InternationalStandardMusicalWorkCode.AsDbValue()),
            new SqlParameter(nameof(WorkDto.ReleasedOn), work.ReleasedOn.AsDbValue()),
            new SqlParameter(nameof(WorkDto.ReleasedOnYearOnly), work.ReleasedOnYearOnly.AsDbValue()),
            new SqlParameter(nameof(WorkDto.SystemEntity), work.SystemEntity.AsDbValue()),
            new SqlParameter(nameof(WorkDto.Enabled), work.Enabled.AsDbValue()),
            resultIdParameter = new SqlParameter($"Result{nameof(WorkDto.Id)}", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output },
            resultCreatedOnParameter = new SqlParameter($"Result{nameof(WorkDto.CreatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
            resultUpdatedOnParameter = new SqlParameter($"Result{nameof(WorkDto.UpdatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_CreateWork]
                @{nameof(WorkDto.Id)},
                @{nameof(WorkDto.Title)},
                @{nameof(WorkDto.Description)},
                @{nameof(WorkDto.DisambiguationText)},
                @{nameof(WorkDto.InternationalStandardMusicalWorkCode)},
                @{nameof(WorkDto.ReleasedOn)},
                @{nameof(WorkDto.ReleasedOnYearOnly)},
                @{nameof(WorkDto.SystemEntity)},
                @{nameof(WorkDto.Enabled)},
                @{resultIdParameter.ParameterName} OUTPUT,
                @{resultCreatedOnParameter.ParameterName} OUTPUT,
                @{resultUpdatedOnParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        work.Id = (Guid)resultIdParameter.Value;
        work.CreatedOn = (DateTimeOffset)resultCreatedOnParameter.Value;
        work.UpdatedOn = (DateTimeOffset)resultUpdatedOnParameter.Value;

        return work;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateWorkAsync(WorkDto work)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(WorkDto.Id), work.Id.AsDbValue()),
            new SqlParameter(nameof(WorkDto.Title), work.Title.AsDbValue()),
            new SqlParameter(nameof(WorkDto.Description), work.Description.AsDbValue()),
            new SqlParameter(nameof(WorkDto.DisambiguationText), work.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(WorkDto.InternationalStandardMusicalWorkCode), work.InternationalStandardMusicalWorkCode.AsDbValue()),
            new SqlParameter(nameof(WorkDto.ReleasedOn), work.ReleasedOn.AsDbValue()),
            new SqlParameter(nameof(WorkDto.ReleasedOnYearOnly), work.ReleasedOnYearOnly.AsDbValue()),
            new SqlParameter(nameof(WorkDto.SystemEntity), work.SystemEntity.AsDbValue()),
            new SqlParameter(nameof(WorkDto.Enabled), work.Enabled.AsDbValue()),
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateWork]
                @{nameof(WorkDto.Id)},
                @{nameof(WorkDto.Title)},
                @{nameof(WorkDto.Description)},
                @{nameof(WorkDto.DisambiguationText)},
                @{nameof(WorkDto.InternationalStandardMusicalWorkCode)},
                @{nameof(WorkDto.ReleasedOn)},
                @{nameof(WorkDto.ReleasedOnYearOnly)},
                @{nameof(WorkDto.SystemEntity)},
                @{nameof(WorkDto.Enabled)},
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteWorkAsync(Guid workId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultRowsDeletedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(WorkDto.Id), workId.AsDbValue()),
            resultRowsDeletedParameter = new SqlParameter("ResultRowsDeleted", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_DeleteWork]
                @{nameof(WorkDto.Id)},
                @{resultRowsDeletedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsDeleted = (int)resultRowsDeletedParameter.Value;
        return rowsDeleted > 0;
    }
}
