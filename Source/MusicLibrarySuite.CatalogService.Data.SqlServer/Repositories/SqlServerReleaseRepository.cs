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
            .Include(release => release.ReleaseRelationships)
            .ThenInclude(releaseRelationship => releaseRelationship.DependentRelease)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .FirstOrDefaultAsync();

        if (release is not null)
        {
            OrderReleaseRelationships(release);
            OrderReleaseMediaCollection(release);
            foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
            {
                OrderReleaseTrackCollection(releaseMedia);
            }
        }

        return release;
    }

    /// <inheritdoc />
    public async Task<ReleaseDto[]> GetReleasesAsync()
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseDto[] releases = await context.Releases.AsNoTracking()
            .Include(release => release.ReleaseRelationships)
            .ThenInclude(releaseRelationship => releaseRelationship.DependentRelease)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ToArrayAsync();

        foreach (ReleaseDto release in releases)
        {
            OrderReleaseRelationships(release);
            OrderReleaseMediaCollection(release);
            foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
            {
                OrderReleaseTrackCollection(releaseMedia);
            }
        }

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
            .Include(release => release.ReleaseRelationships)
            .ThenInclude(releaseRelationship => releaseRelationship.DependentRelease)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ToArrayAsync();

        foreach (ReleaseDto release in releases)
        {
            OrderReleaseRelationships(release);
            OrderReleaseMediaCollection(release);
            foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
            {
                OrderReleaseTrackCollection(releaseMedia);
            }
        }

        return releases;
    }

    /// <inheritdoc />
    public async Task<ReleaseDto[]> GetReleasesAsync(EntityCollectionProcessor<ReleaseDto> collectionProcessor)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseDto[] releases = await collectionProcessor(context.Releases.AsNoTracking())
            .Include(release => release.ReleaseRelationships)
            .ThenInclude(releaseRelationship => releaseRelationship.DependentRelease)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ToArrayAsync();

        foreach (ReleaseDto release in releases)
        {
            OrderReleaseRelationships(release);
            OrderReleaseMediaCollection(release);
            foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
            {
                OrderReleaseTrackCollection(releaseMedia);
            }
        }

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
            .Include(release => release.ReleaseRelationships)
            .ThenInclude(releaseRelationship => releaseRelationship.DependentRelease)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .OrderBy(release => release.Title)
            .Skip(releaseRequest.PageSize * releaseRequest.PageIndex)
            .Take(releaseRequest.PageSize)
            .ToListAsync();

        foreach (ReleaseDto release in releases)
        {
            OrderReleaseRelationships(release);
            OrderReleaseMediaCollection(release);
            foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
            {
                OrderReleaseTrackCollection(releaseMedia);
            }
        }

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

        SetReleaseRelationshipOrders(release.ReleaseRelationships);
        SetReleaseMediaForeignKeys(release.Id, release.ReleaseMediaCollection);
        foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
        {
            SetReleaseTrackForeignKeys(releaseMedia.MediaNumber, releaseMedia.ReleaseId, releaseMedia.ReleaseTrackCollection);
        }

        using var releaseRelationshipsDataTable = new DataTable();
        releaseRelationshipsDataTable.Columns.Add(nameof(ReleaseRelationshipDto.ReleaseId), typeof(Guid));
        releaseRelationshipsDataTable.Columns.Add(nameof(ReleaseRelationshipDto.DependentReleaseId), typeof(Guid));
        releaseRelationshipsDataTable.Columns.Add(nameof(ReleaseRelationshipDto.Name), typeof(string));
        releaseRelationshipsDataTable.Columns.Add(nameof(ReleaseRelationshipDto.Description), typeof(string));
        releaseRelationshipsDataTable.Columns.Add(nameof(ReleaseRelationshipDto.Order), typeof(int));
        foreach (ReleaseRelationshipDto releaseRelationship in release.ReleaseRelationships)
        {
            releaseRelationshipsDataTable.Rows.Add(
                releaseRelationship.ReleaseId.AsDbValue(),
                releaseRelationship.DependentReleaseId.AsDbValue(),
                releaseRelationship.Name.AsDbValue(),
                releaseRelationship.Description.AsDbValue(),
                releaseRelationship.Order.AsDbValue());
        }

        using var releaseMediaCollectionDataTable = new DataTable();
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.MediaNumber), typeof(byte));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.ReleaseId), typeof(Guid));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.Title), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.Description), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.DisambiguationText), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.CatalogNumber), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.MediaFormat), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.TableOfContentsChecksum), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.TableOfContentsChecksumLong), typeof(string));
        foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
        {
            releaseMediaCollectionDataTable.Rows.Add(
                releaseMedia.MediaNumber.AsDbValue(),
                releaseMedia.ReleaseId.AsDbValue(),
                releaseMedia.Title.AsDbValue(),
                releaseMedia.Description.AsDbValue(),
                releaseMedia.DisambiguationText.AsDbValue(),
                releaseMedia.CatalogNumber.AsDbValue(),
                releaseMedia.MediaFormat.AsDbValue(),
                releaseMedia.TableOfContentsChecksum.AsDbValue(),
                releaseMedia.TableOfContentsChecksumLong.AsDbValue());
        }

        using var releaseTrackCollectionDataTable = new DataTable();
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.TrackNumber), typeof(byte));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.MediaNumber), typeof(byte));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.ReleaseId), typeof(Guid));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.Title), typeof(string));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.Description), typeof(string));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.DisambiguationText), typeof(string));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.InternationalStandardRecordingCode), typeof(string));
        foreach (ReleaseTrackDto releaseTrack in release.ReleaseMediaCollection.SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection))
        {
            releaseTrackCollectionDataTable.Rows.Add(
                releaseTrack.TrackNumber.AsDbValue(),
                releaseTrack.MediaNumber.AsDbValue(),
                releaseTrack.ReleaseId.AsDbValue(),
                releaseTrack.Title.AsDbValue(),
                releaseTrack.Description.AsDbValue(),
                releaseTrack.DisambiguationText.AsDbValue(),
                releaseTrack.InternationalStandardRecordingCode.AsDbValue());
        }

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
            new SqlParameter(nameof(ReleaseDto.ReleaseRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseRelationship]", Value = releaseRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseMediaCollection), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseMedia]", Value = releaseMediaCollectionDataTable },
            new SqlParameter(nameof(ReleaseMediaDto.ReleaseTrackCollection), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrack]", Value = releaseTrackCollectionDataTable },
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
                @{nameof(ReleaseDto.ReleaseRelationships)},
                @{nameof(ReleaseDto.ReleaseMediaCollection)},
                @{nameof(ReleaseMediaDto.ReleaseTrackCollection)},
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

        SetReleaseRelationshipOrders(release.ReleaseRelationships);
        SetReleaseMediaForeignKeys(release.Id, release.ReleaseMediaCollection);
        foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
        {
            SetReleaseTrackForeignKeys(releaseMedia.MediaNumber, releaseMedia.ReleaseId, releaseMedia.ReleaseTrackCollection);
        }

        using var releaseRelationshipsDataTable = new DataTable();
        releaseRelationshipsDataTable.Columns.Add(nameof(ReleaseRelationshipDto.ReleaseId), typeof(Guid));
        releaseRelationshipsDataTable.Columns.Add(nameof(ReleaseRelationshipDto.DependentReleaseId), typeof(Guid));
        releaseRelationshipsDataTable.Columns.Add(nameof(ReleaseRelationshipDto.Name), typeof(string));
        releaseRelationshipsDataTable.Columns.Add(nameof(ReleaseRelationshipDto.Description), typeof(string));
        releaseRelationshipsDataTable.Columns.Add(nameof(ReleaseRelationshipDto.Order), typeof(int));
        foreach (ReleaseRelationshipDto releaseRelationship in release.ReleaseRelationships)
        {
            releaseRelationshipsDataTable.Rows.Add(
                releaseRelationship.ReleaseId.AsDbValue(),
                releaseRelationship.DependentReleaseId.AsDbValue(),
                releaseRelationship.Name.AsDbValue(),
                releaseRelationship.Description.AsDbValue(),
                releaseRelationship.Order.AsDbValue());
        }

        using var releaseMediaCollectionDataTable = new DataTable();
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.MediaNumber), typeof(byte));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.ReleaseId), typeof(Guid));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.Title), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.Description), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.DisambiguationText), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.CatalogNumber), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.MediaFormat), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.TableOfContentsChecksum), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.TableOfContentsChecksumLong), typeof(string));
        foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
        {
            releaseMediaCollectionDataTable.Rows.Add(
                releaseMedia.MediaNumber.AsDbValue(),
                releaseMedia.ReleaseId.AsDbValue(),
                releaseMedia.Title.AsDbValue(),
                releaseMedia.Description.AsDbValue(),
                releaseMedia.DisambiguationText.AsDbValue(),
                releaseMedia.CatalogNumber.AsDbValue(),
                releaseMedia.MediaFormat.AsDbValue(),
                releaseMedia.TableOfContentsChecksum.AsDbValue(),
                releaseMedia.TableOfContentsChecksumLong.AsDbValue());
        }

        using var releaseTrackCollectionDataTable = new DataTable();
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.TrackNumber), typeof(byte));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.MediaNumber), typeof(byte));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.ReleaseId), typeof(Guid));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.Title), typeof(string));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.Description), typeof(string));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.DisambiguationText), typeof(string));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.InternationalStandardRecordingCode), typeof(string));
        foreach (ReleaseTrackDto releaseTrack in release.ReleaseMediaCollection.SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection))
        {
            releaseTrackCollectionDataTable.Rows.Add(
                releaseTrack.TrackNumber.AsDbValue(),
                releaseTrack.MediaNumber.AsDbValue(),
                releaseTrack.ReleaseId.AsDbValue(),
                releaseTrack.Title.AsDbValue(),
                releaseTrack.Description.AsDbValue(),
                releaseTrack.DisambiguationText.AsDbValue(),
                releaseTrack.InternationalStandardRecordingCode.AsDbValue());
        }

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
            new SqlParameter(nameof(ReleaseDto.ReleaseRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseRelationship]", Value = releaseRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseMediaCollection), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseMedia]", Value = releaseMediaCollectionDataTable },
            new SqlParameter(nameof(ReleaseMediaDto.ReleaseTrackCollection), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrack]", Value = releaseTrackCollectionDataTable },
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
                @{nameof(ReleaseDto.ReleaseRelationships)},
                @{nameof(ReleaseDto.ReleaseMediaCollection)},
                @{nameof(ReleaseMediaDto.ReleaseTrackCollection)},
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

    private static void OrderReleaseRelationships(ReleaseDto release)
    {
        release.ReleaseRelationships = release.ReleaseRelationships
            .OrderBy(releaseRelationship => releaseRelationship.Order)
            .ToList();
    }

    private static void OrderReleaseMediaCollection(ReleaseDto release)
    {
        release.ReleaseMediaCollection = release.ReleaseMediaCollection
            .OrderBy(releaseMedia => releaseMedia.MediaNumber)
            .ToList();
    }

    private static void OrderReleaseTrackCollection(ReleaseMediaDto releaseMedia)
    {
        releaseMedia.ReleaseTrackCollection = releaseMedia.ReleaseTrackCollection
            .OrderBy(releaseTrack => releaseTrack.TrackNumber)
            .ToList();
    }

    private static void SetReleaseRelationshipOrders(ICollection<ReleaseRelationshipDto> releaseRelationships)
    {
        var i = 0;
        foreach (ReleaseRelationshipDto releaseRelationship in releaseRelationships)
        {
            releaseRelationship.Order = i++;
        }
    }

    private static void SetReleaseMediaForeignKeys(Guid releaseId, IEnumerable<ReleaseMediaDto> releaseMediaCollection)
    {
        foreach (ReleaseMediaDto releaseMedia in releaseMediaCollection)
        {
            releaseMedia.ReleaseId = releaseId;
        }
    }

    private static void SetReleaseTrackForeignKeys(byte mediaNumber, Guid releaseId, IEnumerable<ReleaseTrackDto> releaseTrackCollection)
    {
        foreach (ReleaseTrackDto releaseTrack in releaseTrackCollection)
        {
            releaseTrack.MediaNumber = mediaNumber;
            releaseTrack.ReleaseId = releaseId;
        }
    }
}
