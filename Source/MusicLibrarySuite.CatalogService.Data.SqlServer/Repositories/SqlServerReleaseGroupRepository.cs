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
            .Include(releaseGroup => releaseGroup.ReleaseGroupRelationships)
            .ThenInclude(releaseGroupRelationship => releaseGroupRelationship.DependentReleaseGroup)
            .FirstOrDefaultAsync();

        if (releaseGroup is not null)
        {
            OrderReleaseGroupRelationships(releaseGroup);
        }

        return releaseGroup;
    }

    /// <inheritdoc />
    public async Task<ReleaseGroupDto[]> GetReleaseGroupsAsync()
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseGroupDto[] releaseGroups = await context.ReleaseGroups.AsNoTracking()
            .Include(releaseGroup => releaseGroup.ReleaseGroupRelationships)
            .ThenInclude(releaseGroupRelationship => releaseGroupRelationship.DependentReleaseGroup)
            .ToArrayAsync();

        foreach (ReleaseGroupDto releaseGroup in releaseGroups)
        {
            OrderReleaseGroupRelationships(releaseGroup);
        }

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
            .Include(releaseGroup => releaseGroup.ReleaseGroupRelationships)
            .ThenInclude(releaseGroupRelationship => releaseGroupRelationship.DependentReleaseGroup)
            .ToArrayAsync();

        foreach (ReleaseGroupDto releaseGroup in releaseGroups)
        {
            OrderReleaseGroupRelationships(releaseGroup);
        }

        return releaseGroups;
    }

    /// <inheritdoc />
    public async Task<ReleaseGroupDto[]> GetReleaseGroupsAsync(EntityCollectionProcessor<ReleaseGroupDto> collectionProcessor)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseGroupDto[] releaseGroups = await collectionProcessor(context.ReleaseGroups.AsNoTracking())
            .Include(releaseGroup => releaseGroup.ReleaseGroupRelationships)
            .ThenInclude(releaseGroupRelationship => releaseGroupRelationship.DependentReleaseGroup)
            .ToArrayAsync();

        foreach (ReleaseGroupDto releaseGroup in releaseGroups)
        {
            OrderReleaseGroupRelationships(releaseGroup);
        }

        return releaseGroups;
    }

    /// <inheritdoc />
    public async Task<PageResponseDto<ReleaseGroupDto>> GetReleaseGroupsAsync(ReleaseGroupPageRequestDto releaseGroupPageRequest)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<ReleaseGroupDto> baseCollection = context.ReleaseGroups.AsNoTracking();

        if (releaseGroupPageRequest.Title is not null)
            baseCollection = baseCollection.Where(releaseGroup => releaseGroup.Title.Contains(releaseGroupPageRequest.Title));

        if (releaseGroupPageRequest.Enabled is not null)
            baseCollection = baseCollection.Where(releaseGroup => releaseGroup.Enabled == (bool)releaseGroupPageRequest.Enabled);

        var totalCount = await baseCollection.CountAsync();
        List<ReleaseGroupDto> releaseGroups = await baseCollection
            .Include(releaseGroup => releaseGroup.ReleaseGroupRelationships)
            .ThenInclude(releaseGroupRelationship => releaseGroupRelationship.DependentReleaseGroup)
            .OrderBy(releaseGroup => releaseGroup.Title)
            .Skip(releaseGroupPageRequest.PageSize * releaseGroupPageRequest.PageIndex)
            .Take(releaseGroupPageRequest.PageSize)
            .ToListAsync();

        foreach (ReleaseGroupDto releaseGroup in releaseGroups)
        {
            OrderReleaseGroupRelationships(releaseGroup);
        }

        return new PageResponseDto<ReleaseGroupDto>()
        {
            PageSize = releaseGroupPageRequest.PageSize,
            PageIndex = releaseGroupPageRequest.PageIndex,
            TotalCount = totalCount,
            Items = releaseGroups,
        };
    }

    /// <inheritdoc/>
    public async Task<ReleaseGroupRelationshipDto[]> GetReleaseGroupRelationshipsAsync(Guid releaseGroupId, bool includeReverseRelationships = false)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<ReleaseGroupRelationshipDto> baseCollection = context.ReleaseGroupRelationships.AsNoTracking()
            .Include(releaseGroupRelationship => releaseGroupRelationship.ReleaseGroup)
            .Include(releaseGroupRelationship => releaseGroupRelationship.DependentReleaseGroup);

        ReleaseGroupRelationshipDto[] releaseGroupRelationships = await baseCollection
            .Where(releaseGroupRelationship => releaseGroupRelationship.ReleaseGroupId == releaseGroupId)
            .OrderBy(releaseGroupRelationship => releaseGroupRelationship.Order)
            .ToArrayAsync();

        if (includeReverseRelationships)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            ReleaseGroupRelationshipDto[] reverseRelationships = await baseCollection
                .Where(releaseGroupRelationship => releaseGroupRelationship.DependentReleaseGroupId == releaseGroupId)
                .OrderBy(releaseGroupRelationship => releaseGroupRelationship.ReleaseGroup.Title)
                .ToArrayAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            releaseGroupRelationships = releaseGroupRelationships.Concat(reverseRelationships).ToArray();
        }

        return releaseGroupRelationships;
    }

    /// <inheritdoc />
    public async Task<ReleaseGroupDto> CreateReleaseGroupAsync(ReleaseGroupDto releaseGroup)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SetReleaseGroupRelationshipOrders(releaseGroup.ReleaseGroupRelationships);

        using var releaseGroupRelationshipsDataTable = new DataTable();
        releaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseGroupRelationshipDto.ReleaseGroupId), typeof(Guid));
        releaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseGroupRelationshipDto.DependentReleaseGroupId), typeof(Guid));
        releaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseGroupRelationshipDto.Name), typeof(string));
        releaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseGroupRelationshipDto.Description), typeof(string));
        releaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseGroupRelationshipDto.Order), typeof(int));
        foreach (ReleaseGroupRelationshipDto releaseGroupRelationship in releaseGroup.ReleaseGroupRelationships)
        {
            releaseGroupRelationshipsDataTable.Rows.Add(
                releaseGroupRelationship.ReleaseGroupId.AsDbValue(),
                releaseGroupRelationship.DependentReleaseGroupId.AsDbValue(),
                releaseGroupRelationship.Name.AsDbValue(),
                releaseGroupRelationship.Description.AsDbValue(),
                releaseGroupRelationship.Order.AsDbValue());
        }

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
            new SqlParameter(nameof(ReleaseGroupDto.ReleaseGroupRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseGroupRelationship]", Value = releaseGroupRelationshipsDataTable },
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
                @{nameof(ReleaseGroupDto.ReleaseGroupRelationships)},
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

        SetReleaseGroupRelationshipOrders(releaseGroup.ReleaseGroupRelationships);

        using var releaseGroupRelationshipsDataTable = new DataTable();
        releaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseGroupRelationshipDto.ReleaseGroupId), typeof(Guid));
        releaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseGroupRelationshipDto.DependentReleaseGroupId), typeof(Guid));
        releaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseGroupRelationshipDto.Name), typeof(string));
        releaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseGroupRelationshipDto.Description), typeof(string));
        releaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseGroupRelationshipDto.Order), typeof(int));
        foreach (ReleaseGroupRelationshipDto releaseGroupRelationship in releaseGroup.ReleaseGroupRelationships)
        {
            releaseGroupRelationshipsDataTable.Rows.Add(
                releaseGroupRelationship.ReleaseGroupId.AsDbValue(),
                releaseGroupRelationship.DependentReleaseGroupId.AsDbValue(),
                releaseGroupRelationship.Name.AsDbValue(),
                releaseGroupRelationship.Description.AsDbValue(),
                releaseGroupRelationship.Order.AsDbValue());
        }

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ReleaseGroupDto.Id), releaseGroup.Id.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.Title), releaseGroup.Title.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.Description), releaseGroup.Description.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.DisambiguationText), releaseGroup.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.Enabled), releaseGroup.Enabled.AsDbValue()),
            new SqlParameter(nameof(ReleaseGroupDto.ReleaseGroupRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseGroupRelationship]", Value = releaseGroupRelationshipsDataTable },
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateReleaseGroup]
                @{nameof(ReleaseGroupDto.Id)},
                @{nameof(ReleaseGroupDto.Title)},
                @{nameof(ReleaseGroupDto.Description)},
                @{nameof(ReleaseGroupDto.DisambiguationText)},
                @{nameof(ReleaseGroupDto.Enabled)},
                @{nameof(ReleaseGroupDto.ReleaseGroupRelationships)},
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

    private static void OrderReleaseGroupRelationships(ReleaseGroupDto releaseGroup)
    {
        releaseGroup.ReleaseGroupRelationships = releaseGroup.ReleaseGroupRelationships
            .OrderBy(releaseGroupRelationship => releaseGroupRelationship.Order)
            .ToList();
    }

    private static void SetReleaseGroupRelationshipOrders(ICollection<ReleaseGroupRelationshipDto> releaseGroupRelationships)
    {
        var i = 0;
        foreach (ReleaseGroupRelationshipDto releaseGroupRelationship in releaseGroupRelationships)
        {
            releaseGroupRelationship.Order = i++;
        }
    }
}
