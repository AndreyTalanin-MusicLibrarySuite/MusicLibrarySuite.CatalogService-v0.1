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
/// Represents a SQL Server - specific implementation of the release repository.
/// </summary>
public class SqlServerReleaseRepository : IReleaseRepository
{
    private readonly IDbContextFactory<CatalogServiceDbContext> m_contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerReleaseRepository" /> type using the specified services.
    /// </summary>
    /// <param name="contextFactory">The database context factory.</param>
    public SqlServerReleaseRepository(IDbContextFactory<CatalogServiceDbContext> contextFactory)
    {
        m_contextFactory = contextFactory;
    }

    /// <inheritdoc />
    public async Task<ReleaseDto?> GetReleaseAsync(Guid releaseId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var releaseIdParameter = new SqlParameter("ReleaseId", releaseId.AsDbValue());

        var query = $"SELECT * FROM [dbo].[ufn_GetRelease] (@{releaseIdParameter.ParameterName})";

        ReleaseDto? release = await context.Releases.FromSqlRaw(query, releaseIdParameter).AsNoTracking()
            .FirstOrDefaultAsync();

        return release;
    }

    /// <inheritdoc />
    public async Task<ReleaseDto[]> GetReleasesAsync()
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseDto[] releases = await context.Releases.AsNoTracking()
            .ToArrayAsync();

        return releases;
    }

    /// <inheritdoc />
    public async Task<ReleaseDto[]> GetReleasesAsync(IEnumerable<Guid> releaseIds)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        using var releaseIdsDataTable = new DataTable();
        releaseIdsDataTable.Columns.Add("Value", typeof(Guid));
        foreach (Guid releaseId in releaseIds)
            releaseIdsDataTable.Rows.Add(releaseId.AsDbValue());

        var releaseIdsParameter = new SqlParameter("ReleaseIds", SqlDbType.Structured) { TypeName = "[dbo].[GuidArray]", Value = releaseIdsDataTable };

        var query = $"SELECT * FROM [dbo].[ufn_GetReleases] (@{releaseIdsParameter.ParameterName})";

        ReleaseDto[] releases = await context.Releases.FromSqlRaw(query, releaseIdsParameter).AsNoTracking()
            .ToArrayAsync();

        return releases;
    }

    /// <inheritdoc />
    public async Task<ReleaseDto[]> GetReleasesAsync(EntityCollectionProcessor<ReleaseDto> collectionProcessor)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseDto[] releases = await collectionProcessor(context.Releases.AsNoTracking())
            .ToArrayAsync();

        return releases;
    }

    /// <inheritdoc />
    public async Task<PageResponseDto<ReleaseDto>> GetReleasesAsync(ReleaseRequestDto releaseRequest)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<ReleaseDto> baseCollection = context.Releases.AsNoTracking();

        if (releaseRequest.Title is not null)
            baseCollection = baseCollection.Where(release => release.Title.Contains(releaseRequest.Title));

        if (releaseRequest.Enabled is not null)
            baseCollection = baseCollection.Where(release => release.Enabled == (bool)releaseRequest.Enabled);

        var totalCount = await baseCollection.CountAsync();
        List<ReleaseDto> releases = await baseCollection
            .OrderBy(release => release.Title)
            .Skip(releaseRequest.PageSize * releaseRequest.PageIndex)
            .Take(releaseRequest.PageSize)
            .ToListAsync();

        return new PageResponseDto<ReleaseDto>()
        {
            PageSize = releaseRequest.PageSize,
            PageIndex = releaseRequest.PageIndex,
            TotalCount = totalCount,
            Items = releases,
        };
    }

    /// <inheritdoc />
    public async Task<ReleaseDto> CreateReleaseAsync(ReleaseDto release)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultIdParameter;
        SqlParameter resultCreatedOnParameter;
        SqlParameter resultUpdatedOnParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ReleaseDto.Id), release.Id.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Title), release.Title.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Description), release.Description.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.DisambiguationText), release.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Barcode), release.Barcode.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.CatalogNumber), release.CatalogNumber.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.MediaFormat), release.MediaFormat.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.PublishFormat), release.PublishFormat.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.ReleasedOn), release.ReleasedOn.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.ReleasedOnYearOnly), release.ReleasedOnYearOnly.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Enabled), release.Enabled.AsDbValue()),
            resultIdParameter = new SqlParameter($"Result{nameof(ReleaseDto.Id)}", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output },
            resultCreatedOnParameter = new SqlParameter($"Result{nameof(ReleaseDto.CreatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
            resultUpdatedOnParameter = new SqlParameter($"Result{nameof(ReleaseDto.UpdatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_CreateRelease]
                @{nameof(ReleaseDto.Id)},
                @{nameof(ReleaseDto.Title)},
                @{nameof(ReleaseDto.Description)},
                @{nameof(ReleaseDto.DisambiguationText)},
                @{nameof(ReleaseDto.Barcode)},
                @{nameof(ReleaseDto.CatalogNumber)},
                @{nameof(ReleaseDto.MediaFormat)},
                @{nameof(ReleaseDto.PublishFormat)},
                @{nameof(ReleaseDto.ReleasedOn)},
                @{nameof(ReleaseDto.ReleasedOnYearOnly)},
                @{nameof(ReleaseDto.Enabled)},
                @{resultIdParameter.ParameterName} OUTPUT,
                @{resultCreatedOnParameter.ParameterName} OUTPUT,
                @{resultUpdatedOnParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        release.Id = (Guid)resultIdParameter.Value;
        release.CreatedOn = (DateTimeOffset)resultCreatedOnParameter.Value;
        release.UpdatedOn = (DateTimeOffset)resultUpdatedOnParameter.Value;

        return release;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseAsync(ReleaseDto release)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ReleaseDto.Id), release.Id.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Title), release.Title.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Description), release.Description.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.DisambiguationText), release.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Barcode), release.Barcode.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.CatalogNumber), release.CatalogNumber.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.MediaFormat), release.MediaFormat.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.PublishFormat), release.PublishFormat.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.ReleasedOn), release.ReleasedOn.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.ReleasedOnYearOnly), release.ReleasedOnYearOnly.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Enabled), release.Enabled.AsDbValue()),
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateRelease]
                @{nameof(ReleaseDto.Id)},
                @{nameof(ReleaseDto.Title)},
                @{nameof(ReleaseDto.Description)},
                @{nameof(ReleaseDto.DisambiguationText)},
                @{nameof(ReleaseDto.Barcode)},
                @{nameof(ReleaseDto.CatalogNumber)},
                @{nameof(ReleaseDto.MediaFormat)},
                @{nameof(ReleaseDto.PublishFormat)},
                @{nameof(ReleaseDto.ReleasedOn)},
                @{nameof(ReleaseDto.ReleasedOnYearOnly)},
                @{nameof(ReleaseDto.Enabled)},
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteReleaseAsync(Guid releaseId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultRowsDeletedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ReleaseDto.Id), releaseId.AsDbValue()),
            resultRowsDeletedParameter = new SqlParameter("ResultRowsDeleted", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_DeleteRelease]
                @{nameof(ReleaseDto.Id)},
                @{resultRowsDeletedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsDeleted = (int)resultRowsDeletedParameter.Value;
        return rowsDeleted > 0;
    }
}
