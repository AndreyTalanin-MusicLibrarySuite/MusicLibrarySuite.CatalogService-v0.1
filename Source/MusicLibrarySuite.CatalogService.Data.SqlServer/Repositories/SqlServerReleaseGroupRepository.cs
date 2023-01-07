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
/// Represents a SQL Server - specific implementation of the release group repository.
/// </summary>
public class SqlServerReleaseGroupRepository : IReleaseGroupRepository
{
    private readonly IDbContextFactory<CatalogServiceDbContext> m_contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerReleaseGroupRepository" /> type using the specified services.
    /// </summary>
    /// <param name="contextFactory">The database context factory.</param>
    public SqlServerReleaseGroupRepository(IDbContextFactory<CatalogServiceDbContext> contextFactory)
    {
        m_contextFactory = contextFactory;
    }

    /// <inheritdoc />
    public async Task<ReleaseGroupDto?> GetReleaseGroupAsync(Guid releaseGroupId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var releaseGroupIdParameter = new SqlParameter("ReleaseGroupId", releaseGroupId.AsDbValue());

        var query = $"SELECT * FROM [dbo].[ufn_GetReleaseGroup] (@{releaseGroupIdParameter.ParameterName})";

        ReleaseGroupDto? releaseGroup = await context.ReleaseGroups.FromSqlRaw(query, releaseGroupIdParameter).AsNoTracking()
            .FirstOrDefaultAsync();

        return releaseGroup;
    }

    /// <inheritdoc />
    public async Task<ReleaseGroupDto[]> GetReleaseGroupsAsync()
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseGroupDto[] releaseGroups = await context.ReleaseGroups.AsNoTracking()
            .ToArrayAsync();

        return releaseGroups;
    }

    /// <inheritdoc />
    public async Task<ReleaseGroupDto[]> GetReleaseGroupsAsync(IEnumerable<Guid> releaseGroupIds)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        using var releaseGroupIdsDataTable = new DataTable();
        releaseGroupIdsDataTable.Columns.Add("Value", typeof(Guid));
        foreach (Guid releaseGroupId in releaseGroupIds)
            releaseGroupIdsDataTable.Rows.Add(releaseGroupId.AsDbValue());

        var releaseGroupIdsParameter = new SqlParameter("ReleaseGroupIds", SqlDbType.Structured) { TypeName = "[dbo].[GuidArray]", Value = releaseGroupIdsDataTable };

        var query = $"SELECT * FROM [dbo].[ufn_GetReleaseGroups] (@{releaseGroupIdsParameter.ParameterName})";

        ReleaseGroupDto[] releaseGroups = await context.ReleaseGroups.FromSqlRaw(query, releaseGroupIdsParameter).AsNoTracking()
            .ToArrayAsync();

        return releaseGroups;
    }

    /// <inheritdoc />
    public async Task<ReleaseGroupDto[]> GetReleaseGroupsAsync(EntityCollectionProcessor<ReleaseGroupDto> collectionProcessor)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseGroupDto[] releaseGroups = await collectionProcessor(context.ReleaseGroups.AsNoTracking())
            .ToArrayAsync();

        return releaseGroups;
    }

    /// <inheritdoc />
    public async Task<PageResponseDto<ReleaseGroupDto>> GetReleaseGroupsAsync(ReleaseGroupRequestDto releaseGroupRequest)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<ReleaseGroupDto> baseCollection = context.ReleaseGroups.AsNoTracking();

        if (releaseGroupRequest.Title is not null)
            baseCollection = baseCollection.Where(releaseGroup => releaseGroup.Title.Contains(releaseGroupRequest.Title));

        if (releaseGroupRequest.Enabled is not null)
            baseCollection = baseCollection.Where(releaseGroup => releaseGroup.Enabled == (bool)releaseGroupRequest.Enabled);

        var totalCount = await baseCollection.CountAsync();
        List<ReleaseGroupDto> releaseGroups = await baseCollection
            .OrderBy(releaseGroup => releaseGroup.Title)
            .Skip(releaseGroupRequest.PageSize * releaseGroupRequest.PageIndex)
            .Take(releaseGroupRequest.PageSize)
            .ToListAsync();

        return new PageResponseDto<ReleaseGroupDto>()
        {
            PageSize = releaseGroupRequest.PageSize,
            PageIndex = releaseGroupRequest.PageIndex,
            TotalCount = totalCount,
            Items = releaseGroups,
        };
    }

    /// <inheritdoc />
    public async Task<ReleaseGroupDto> CreateReleaseGroupAsync(ReleaseGroupDto releaseGroup)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultIdParameter;
        SqlParameter resultCreatedOnParameter;
        SqlParameter resultUpdatedOnParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ReleaseGroupDto.Id), releaseGroup.Id.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.Title), releaseGroup.Title.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.Description), releaseGroup.Description.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.DisambiguationText), releaseGroup.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.Enabled), releaseGroup.Enabled.AsDbValue()),
            resultIdParameter = new SqlParameter($"Result{nameof(ReleaseGroupDto.Id)}", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output },
            resultCreatedOnParameter = new SqlParameter($"Result{nameof(ReleaseGroupDto.CreatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
            resultUpdatedOnParameter = new SqlParameter($"Result{nameof(ReleaseGroupDto.UpdatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_CreateReleaseGroup]
                @{nameof(ReleaseGroupDto.Id)},
                @{nameof(ReleaseGroupDto.Title)},
                @{nameof(ReleaseGroupDto.Description)},
                @{nameof(ReleaseGroupDto.DisambiguationText)},
                @{nameof(ReleaseGroupDto.Enabled)},
                @{resultIdParameter.ParameterName} OUTPUT,
                @{resultCreatedOnParameter.ParameterName} OUTPUT,
                @{resultUpdatedOnParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        releaseGroup.Id = (Guid)resultIdParameter.Value;
        releaseGroup.CreatedOn = (DateTimeOffset)resultCreatedOnParameter.Value;
        releaseGroup.UpdatedOn = (DateTimeOffset)resultUpdatedOnParameter.Value;

        return releaseGroup;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseGroupAsync(ReleaseGroupDto releaseGroup)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ReleaseGroupDto.Id), releaseGroup.Id.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.Title), releaseGroup.Title.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.Description), releaseGroup.Description.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.DisambiguationText), releaseGroup.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.Enabled), releaseGroup.Enabled.AsDbValue()),
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateReleaseGroup]
                @{nameof(ReleaseGroupDto.Id)},
                @{nameof(ReleaseGroupDto.Title)},
                @{nameof(ReleaseGroupDto.Description)},
                @{nameof(ReleaseGroupDto.DisambiguationText)},
                @{nameof(ReleaseGroupDto.Enabled)},
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteReleaseGroupAsync(Guid releaseGroupId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultRowsDeletedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ReleaseGroupDto.Id), releaseGroupId.AsDbValue()),
            resultRowsDeletedParameter = new SqlParameter("ResultRowsDeleted", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_DeleteReleaseGroup]
                @{nameof(ReleaseGroupDto.Id)},
                @{resultRowsDeletedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsDeleted = (int)resultRowsDeletedParameter.Value;
        return rowsDeleted > 0;
    }
}
