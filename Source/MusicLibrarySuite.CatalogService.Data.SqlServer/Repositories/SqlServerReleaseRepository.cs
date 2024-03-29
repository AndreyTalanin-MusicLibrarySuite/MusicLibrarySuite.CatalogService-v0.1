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
/// Represents a SQL-Server-specific implementation of the release repository.
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

        ReleaseDto? release = await context.Releases
            .FromSqlRaw(query, releaseIdParameter)
            .Include(release => release.ReleaseRelationships)
            .ThenInclude(releaseRelationship => releaseRelationship.DependentRelease)
            .Include(release => release.ReleaseToProductRelationships)
            .ThenInclude(releaseToProductRelationship => releaseToProductRelationship.Product)
            .Include(release => release.ReleaseToReleaseGroupRelationships)
            .ThenInclude(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.ReleaseGroup)
            .Include(release => release.ReleaseArtists)
            .ThenInclude(releaseArtist => releaseArtist.Artist)
            .Include(release => release.ReleaseFeaturedArtists)
            .ThenInclude(releaseFeaturedArtist => releaseFeaturedArtist.Artist)
            .Include(release => release.ReleasePerformers)
            .ThenInclude(releasePerformer => releasePerformer.Artist)
            .Include(release => release.ReleaseComposers)
            .ThenInclude(releaseComposer => releaseComposer.Artist)
            .Include(release => release.ReleaseGenres)
            .ThenInclude(releaseGenre => releaseGenre.Genre)
            .Include(release => release.ReleaseMediaCollection)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseMediaToProductRelationships)
            .ThenInclude(releaseMediaToProductRelationship => releaseMediaToProductRelationship.Product)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ThenInclude(releaseTrack => releaseTrack.ReleaseTrackToProductRelationships)
            .ThenInclude(releaseTrackToProductRelationship => releaseTrackToProductRelationship.Product)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ThenInclude(releaseTrack => releaseTrack.ReleaseTrackToWorkRelationships)
            .ThenInclude(releaseTrackToWorkRelationship => releaseTrackToWorkRelationship.Work)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ThenInclude(releaseTrack => releaseTrack.ReleaseTrackArtists)
            .ThenInclude(releaseTrackArtist => releaseTrackArtist.Artist)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ThenInclude(releaseTrack => releaseTrack.ReleaseTrackFeaturedArtists)
            .ThenInclude(releaseTrackFeaturedArtist => releaseTrackFeaturedArtist.Artist)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ThenInclude(releaseTrack => releaseTrack.ReleaseTrackPerformers)
            .ThenInclude(releaseTrackPerformer => releaseTrackPerformer.Artist)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ThenInclude(releaseTrack => releaseTrack.ReleaseTrackComposers)
            .ThenInclude(releaseTrackComposer => releaseTrackComposer.Artist)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ThenInclude(releaseTrack => releaseTrack.ReleaseTrackGenres)
            .ThenInclude(releaseTrackGenre => releaseTrackGenre.Genre)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync();

        if (release is not null)
        {
            OrderReleaseRelationships(release);
            OrderReleaseToProductRelationships(release);
            OrderReleaseToReleaseGroupRelationships(release);
            OrderReleaseArtists(release);
            OrderReleaseFeaturedArtists(release);
            OrderReleasePerformers(release);
            OrderReleaseComposers(release);
            OrderReleaseGenres(release);

            OrderReleaseMediaCollection(release);
            foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
            {
                OrderReleaseMediaToProductRelationships(releaseMedia);

                OrderReleaseTrackCollection(releaseMedia);
                foreach (ReleaseTrackDto releaseTrack in releaseMedia.ReleaseTrackCollection)
                {
                    OrderReleaseTrackToProductRelationships(releaseTrack);
                    OrderReleaseTrackToWorkRelationships(releaseTrack);
                    OrderReleaseTrackArtists(releaseTrack);
                    OrderReleaseTrackFeaturedArtists(releaseTrack);
                    OrderReleaseTrackPerformers(releaseTrack);
                    OrderReleaseTrackComposers(releaseTrack);
                    OrderReleaseTrackGenres(releaseTrack);
                }
            }
        }

        return release;
    }

    /// <inheritdoc />
    public async Task<ReleaseDto[]> GetReleasesAsync()
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseDto[] releases = await context.Releases
            .AsNoTracking()
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

        ReleaseDto[] releases = await context.Releases
            .FromSqlRaw(query, releaseIdsParameter)
            .AsNoTracking()
            .ToArrayAsync();

        return releases;
    }

    /// <inheritdoc />
    public async Task<ReleaseDto[]> GetReleasesAsync(EntityCollectionProcessor<ReleaseDto> collectionProcessor)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseDto[] releases = await collectionProcessor(context.Releases)
            .AsNoTracking()
            .ToArrayAsync();

        return releases;
    }

    /// <inheritdoc />
    public async Task<PageResponseDto<ReleaseDto>> GetReleasesAsync(ReleasePageRequestDto releasePageRequest)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<ReleaseDto> baseCollection = context.Releases;

        if (releasePageRequest.Title is not null)
            baseCollection = baseCollection.Where(release => release.Title.Contains(releasePageRequest.Title));

        if (releasePageRequest.Enabled is not null)
            baseCollection = baseCollection.Where(release => release.Enabled == (bool)releasePageRequest.Enabled);

        var totalCount = await baseCollection.CountAsync();
        List<ReleaseDto> releases = await baseCollection
            .OrderBy(release => release.Title)
            .Skip(releasePageRequest.PageSize * releasePageRequest.PageIndex)
            .Take(releasePageRequest.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PageResponseDto<ReleaseDto>()
        {
            PageSize = releasePageRequest.PageSize,
            PageIndex = releasePageRequest.PageIndex,
            TotalCount = totalCount,
            Items = releases,
        };
    }

    /// <inheritdoc />
    public async Task<ReleaseRelationshipDto[]> GetReleaseRelationshipsAsync(Guid releaseId, bool includeReverseRelationships = false)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<ReleaseRelationshipDto> baseCollection = context.ReleaseRelationships.AsNoTracking()
            .Include(releaseRelationship => releaseRelationship.Release)
            .Include(releaseRelationship => releaseRelationship.DependentRelease);

        ReleaseRelationshipDto[] releaseRelationships = await baseCollection
            .Where(releaseRelationship => releaseRelationship.ReleaseId == releaseId)
            .OrderBy(releaseRelationship => releaseRelationship.Order)
            .ToArrayAsync();

        if (includeReverseRelationships)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            ReleaseRelationshipDto[] reverseRelationships = await baseCollection
                .Where(releaseRelationship => releaseRelationship.DependentReleaseId == releaseId)
                .OrderBy(releaseRelationship => releaseRelationship.Release.Title)
                .ToArrayAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            releaseRelationships = releaseRelationships.Concat(reverseRelationships).ToArray();
        }

        return releaseRelationships;
    }

    /// <inheritdoc />
    public async Task<ReleaseToProductRelationshipDto[]> GetReleaseToProductRelationshipsAsync(Guid releaseId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseToProductRelationshipDto[] releaseToProductRelationships = await context.ReleaseToProductRelationships.AsNoTracking()
            .Include(releaseToProductRelationship => releaseToProductRelationship.Release)
            .Include(releaseToProductRelationship => releaseToProductRelationship.Product)
            .Where(releaseToProductRelationship => releaseToProductRelationship.ReleaseId == releaseId)
            .OrderBy(releaseToProductRelationship => releaseToProductRelationship.Order)
            .ToArrayAsync();

        return releaseToProductRelationships;
    }

    /// <inheritdoc />
    public async Task<ReleaseToProductRelationshipDto[]> GetReleaseToProductRelationshipsByProductAsync(Guid productId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseToProductRelationshipDto[] releaseToProductRelationships = await context.ReleaseToProductRelationships.AsNoTracking()
            .Include(releaseToProductRelationship => releaseToProductRelationship.Release)
            .Include(releaseToProductRelationship => releaseToProductRelationship.Product)
            .Where(releaseToProductRelationship => releaseToProductRelationship.ProductId == productId)
            .OrderBy(releaseToProductRelationship => releaseToProductRelationship.ReferenceOrder)
            .ToArrayAsync();

        return releaseToProductRelationships;
    }

    /// <inheritdoc />
    public async Task<ReleaseToReleaseGroupRelationshipDto[]> GetReleaseToReleaseGroupRelationshipsAsync(Guid releaseId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseToReleaseGroupRelationshipDto[] releaseToReleaseGroupRelationships = await context.ReleaseToReleaseGroupRelationships.AsNoTracking()
            .Include(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.Release)
            .Include(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.ReleaseGroup)
            .Where(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.ReleaseId == releaseId)
            .OrderBy(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.Order)
            .ToArrayAsync();

        return releaseToReleaseGroupRelationships;
    }

    /// <inheritdoc />
    public async Task<ReleaseToReleaseGroupRelationshipDto[]> GetReleaseToReleaseGroupRelationshipsByReleaseGroupAsync(Guid releaseGroupId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseToReleaseGroupRelationshipDto[] releaseToReleaseGroupRelationships = await context.ReleaseToReleaseGroupRelationships.AsNoTracking()
            .Include(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.Release)
            .Include(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.ReleaseGroup)
            .Where(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.ReleaseGroupId == releaseGroupId)
            .OrderBy(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.ReferenceOrder)
            .ToArrayAsync();

        return releaseToReleaseGroupRelationships;
    }

    /// <inheritdoc />
    public async Task<ReleaseMediaToProductRelationshipDto[]> GetReleaseMediaToProductRelationshipsAsync(Guid releaseId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseMediaToProductRelationshipDto[] releaseMediaToProductRelationships = await context.ReleaseMediaToProductRelationships.AsNoTracking()
            .Include(releaseMediaToProductRelationship => releaseMediaToProductRelationship.ReleaseMedia)
            .Include(releaseMediaToProductRelationship => releaseMediaToProductRelationship.Product)
            .Where(releaseMediaToProductRelationship => releaseMediaToProductRelationship.ReleaseId == releaseId)
            .OrderBy(releaseMediaToProductRelationship => releaseMediaToProductRelationship.Order)
            .ToArrayAsync();

        return releaseMediaToProductRelationships;
    }

    /// <inheritdoc />
    public async Task<ReleaseMediaToProductRelationshipDto[]> GetReleaseMediaToProductRelationshipsByProductAsync(Guid productId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseMediaToProductRelationshipDto[] releaseMediaToProductRelationships = await context.ReleaseMediaToProductRelationships.AsNoTracking()
            .Include(releaseMediaToProductRelationship => releaseMediaToProductRelationship.ReleaseMedia)
            .Include(releaseMediaToProductRelationship => releaseMediaToProductRelationship.Product)
            .Where(releaseMediaToProductRelationship => releaseMediaToProductRelationship.ProductId == productId)
            .OrderBy(releaseMediaToProductRelationship => releaseMediaToProductRelationship.ReferenceOrder)
            .ToArrayAsync();

        return releaseMediaToProductRelationships;
    }

    /// <inheritdoc />
    public async Task<ReleaseTrackToProductRelationshipDto[]> GetReleaseTrackToProductRelationshipsAsync(Guid releaseId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseTrackToProductRelationshipDto[] releaseTrackToProductRelationships = await context.ReleaseTrackToProductRelationships.AsNoTracking()
            .Include(releaseTrackToProductRelationship => releaseTrackToProductRelationship.ReleaseTrack)
            .Include(releaseTrackToProductRelationship => releaseTrackToProductRelationship.Product)
            .Where(releaseTrackToProductRelationship => releaseTrackToProductRelationship.ReleaseId == releaseId)
            .OrderBy(releaseTrackToProductRelationship => releaseTrackToProductRelationship.Order)
            .ToArrayAsync();

        return releaseTrackToProductRelationships;
    }

    /// <inheritdoc />
    public async Task<ReleaseTrackToProductRelationshipDto[]> GetReleaseTrackToProductRelationshipsByProductAsync(Guid productId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseTrackToProductRelationshipDto[] releaseTrackToProductRelationships = await context.ReleaseTrackToProductRelationships.AsNoTracking()
            .Include(releaseTrackToProductRelationship => releaseTrackToProductRelationship.ReleaseTrack)
            .Include(releaseTrackToProductRelationship => releaseTrackToProductRelationship.Product)
            .Where(releaseTrackToProductRelationship => releaseTrackToProductRelationship.ProductId == productId)
            .OrderBy(releaseTrackToProductRelationship => releaseTrackToProductRelationship.ReferenceOrder)
            .ToArrayAsync();

        return releaseTrackToProductRelationships;
    }

    /// <inheritdoc />
    public async Task<ReleaseTrackToWorkRelationshipDto[]> GetReleaseTrackToWorkRelationshipsAsync(Guid releaseId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseTrackToWorkRelationshipDto[] releaseTrackToWorkRelationships = await context.ReleaseTrackToWorkRelationships.AsNoTracking()
            .Include(releaseTrackToWorkRelationship => releaseTrackToWorkRelationship.ReleaseTrack)
            .Include(releaseTrackToWorkRelationship => releaseTrackToWorkRelationship.Work)
            .Where(releaseTrackToWorkRelationship => releaseTrackToWorkRelationship.ReleaseId == releaseId)
            .OrderBy(releaseTrackToWorkRelationship => releaseTrackToWorkRelationship.Order)
            .ToArrayAsync();

        return releaseTrackToWorkRelationships;
    }

    /// <inheritdoc />
    public async Task<ReleaseTrackToWorkRelationshipDto[]> GetReleaseTrackToWorkRelationshipsByWorkAsync(Guid workId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ReleaseTrackToWorkRelationshipDto[] releaseTrackToWorkRelationships = await context.ReleaseTrackToWorkRelationships.AsNoTracking()
            .Include(releaseTrackToWorkRelationship => releaseTrackToWorkRelationship.ReleaseTrack)
            .Include(releaseTrackToWorkRelationship => releaseTrackToWorkRelationship.Work)
            .Where(releaseTrackToWorkRelationship => releaseTrackToWorkRelationship.WorkId == workId)
            .OrderBy(releaseTrackToWorkRelationship => releaseTrackToWorkRelationship.ReferenceOrder)
            .ToArrayAsync();

        return releaseTrackToWorkRelationships;
    }

    /// <inheritdoc />
    public async Task<ReleaseDto> CreateReleaseAsync(ReleaseDto release)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SetReleaseRelationshipOrders(release.ReleaseRelationships);
        SetReleaseToProductRelationshipOrders(release.ReleaseToProductRelationships);
        SetReleaseToReleaseGroupRelationshipOrders(release.ReleaseToReleaseGroupRelationships);
        SetReleaseArtistOrders(release.ReleaseArtists);
        SetReleaseFeaturedArtistOrders(release.ReleaseFeaturedArtists);
        SetReleasePerformerOrders(release.ReleasePerformers);
        SetReleaseComposerOrders(release.ReleaseComposers);
        SetReleaseGenreOrders(release.ReleaseGenres);

        SetReleaseMediaForeignKeys(release.Id, release.ReleaseMediaCollection);
        foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
        {
            SetReleaseMediaToProductRelationshipOrders(releaseMedia.MediaNumber, releaseMedia.ReleaseId, releaseMedia.ReleaseMediaToProductRelationships);

            SetReleaseTrackForeignKeys(releaseMedia.MediaNumber, releaseMedia.ReleaseId, releaseMedia.ReleaseTrackCollection);
            foreach (ReleaseTrackDto releaseTrack in releaseMedia.ReleaseTrackCollection)
            {
                SetReleaseTrackToProductRelationshipOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackToProductRelationships);
                SetReleaseTrackToWorkRelationshipOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackToWorkRelationships);
                SetReleaseTrackArtistOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackArtists);
                SetReleaseTrackFeaturedArtistOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackFeaturedArtists);
                SetReleaseTrackPerformerOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackPerformers);
                SetReleaseTrackComposerOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackComposers);
                SetReleaseTrackGenreOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackGenres);
            }
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

        using var releaseToProductRelationshipsDataTable = new DataTable();
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.ReleaseId), typeof(Guid));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.ProductId), typeof(Guid));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.Name), typeof(string));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.Description), typeof(string));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.Order), typeof(int));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseToProductRelationshipDto releaseToProductRelationship in release.ReleaseToProductRelationships)
        {
            releaseToProductRelationshipsDataTable.Rows.Add(
                releaseToProductRelationship.ReleaseId.AsDbValue(),
                releaseToProductRelationship.ProductId.AsDbValue(),
                releaseToProductRelationship.Name.AsDbValue(),
                releaseToProductRelationship.Description.AsDbValue(),
                releaseToProductRelationship.Order.AsDbValue(),
                releaseToProductRelationship.ReferenceOrder.AsDbValue());
        }

        using var releaseToReleaseGroupRelationshipsDataTable = new DataTable();
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.ReleaseId), typeof(Guid));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.ReleaseGroupId), typeof(Guid));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.Name), typeof(string));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.Description), typeof(string));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.Order), typeof(int));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseToReleaseGroupRelationshipDto releaseToReleaseGroupRelationship in release.ReleaseToReleaseGroupRelationships)
        {
            releaseToReleaseGroupRelationshipsDataTable.Rows.Add(
                releaseToReleaseGroupRelationship.ReleaseId.AsDbValue(),
                releaseToReleaseGroupRelationship.ReleaseGroupId.AsDbValue(),
                releaseToReleaseGroupRelationship.Name.AsDbValue(),
                releaseToReleaseGroupRelationship.Description.AsDbValue(),
                releaseToReleaseGroupRelationship.Order.AsDbValue(),
                releaseToReleaseGroupRelationship.ReferenceOrder.AsDbValue());
        }

        using var releaseArtistsDataTable = new DataTable();
        releaseArtistsDataTable.Columns.Add(nameof(ReleaseArtistDto.ReleaseId), typeof(Guid));
        releaseArtistsDataTable.Columns.Add(nameof(ReleaseArtistDto.ArtistId), typeof(Guid));
        releaseArtistsDataTable.Columns.Add(nameof(ReleaseArtistDto.Order), typeof(int));
        foreach (ReleaseArtistDto releaseArtist in release.ReleaseArtists)
        {
            releaseArtistsDataTable.Rows.Add(
                releaseArtist.ReleaseId.AsDbValue(),
                releaseArtist.ArtistId.AsDbValue(),
                releaseArtist.Order.AsDbValue());
        }

        using var releaseFeaturedArtistsDataTable = new DataTable();
        releaseFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseFeaturedArtistDto.ReleaseId), typeof(Guid));
        releaseFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseFeaturedArtistDto.ArtistId), typeof(Guid));
        releaseFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseFeaturedArtistDto.Order), typeof(int));
        foreach (ReleaseFeaturedArtistDto releaseFeaturedArtist in release.ReleaseFeaturedArtists)
        {
            releaseFeaturedArtistsDataTable.Rows.Add(
                releaseFeaturedArtist.ReleaseId.AsDbValue(),
                releaseFeaturedArtist.ArtistId.AsDbValue(),
                releaseFeaturedArtist.Order.AsDbValue());
        }

        using var releasePerformersDataTable = new DataTable();
        releasePerformersDataTable.Columns.Add(nameof(ReleasePerformerDto.ReleaseId), typeof(Guid));
        releasePerformersDataTable.Columns.Add(nameof(ReleasePerformerDto.ArtistId), typeof(Guid));
        releasePerformersDataTable.Columns.Add(nameof(ReleasePerformerDto.Order), typeof(int));
        foreach (ReleasePerformerDto releasePerformer in release.ReleasePerformers)
        {
            releasePerformersDataTable.Rows.Add(
                releasePerformer.ReleaseId.AsDbValue(),
                releasePerformer.ArtistId.AsDbValue(),
                releasePerformer.Order.AsDbValue());
        }

        using var releaseComposersDataTable = new DataTable();
        releaseComposersDataTable.Columns.Add(nameof(ReleaseComposerDto.ReleaseId), typeof(Guid));
        releaseComposersDataTable.Columns.Add(nameof(ReleaseComposerDto.ArtistId), typeof(Guid));
        releaseComposersDataTable.Columns.Add(nameof(ReleaseComposerDto.Order), typeof(int));
        foreach (ReleaseComposerDto releaseComposer in release.ReleaseComposers)
        {
            releaseComposersDataTable.Rows.Add(
                releaseComposer.ReleaseId.AsDbValue(),
                releaseComposer.ArtistId.AsDbValue(),
                releaseComposer.Order.AsDbValue());
        }

        using var releaseGenresDataTable = new DataTable();
        releaseGenresDataTable.Columns.Add(nameof(ReleaseGenreDto.ReleaseId), typeof(Guid));
        releaseGenresDataTable.Columns.Add(nameof(ReleaseGenreDto.GenreId), typeof(Guid));
        releaseGenresDataTable.Columns.Add(nameof(ReleaseGenreDto.Order), typeof(int));
        foreach (ReleaseGenreDto releaseGenre in release.ReleaseGenres)
        {
            releaseGenresDataTable.Rows.Add(
                releaseGenre.ReleaseId.AsDbValue(),
                releaseGenre.GenreId.AsDbValue(),
                releaseGenre.Order.AsDbValue());
        }

        using var releaseMediaCollectionDataTable = new DataTable();
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.MediaNumber), typeof(byte));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.ReleaseId), typeof(Guid));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.Title), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.Description), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.DisambiguationText), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.MediaFormat), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.CatalogNumber), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.FreeDbChecksum), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.MusicBrainzChecksum), typeof(string));
        foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
        {
            releaseMediaCollectionDataTable.Rows.Add(
                releaseMedia.MediaNumber.AsDbValue(),
                releaseMedia.ReleaseId.AsDbValue(),
                releaseMedia.Title.AsDbValue(),
                releaseMedia.Description.AsDbValue(),
                releaseMedia.DisambiguationText.AsDbValue(),
                releaseMedia.MediaFormat.AsDbValue(),
                releaseMedia.CatalogNumber.AsDbValue(),
                releaseMedia.FreeDbChecksum.AsDbValue(),
                releaseMedia.MusicBrainzChecksum.AsDbValue());
        }

        using var releaseMediaToProductRelationshipsDataTable = new DataTable();
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.MediaNumber), typeof(byte));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.ReleaseId), typeof(Guid));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.ProductId), typeof(Guid));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.Name), typeof(string));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.Description), typeof(string));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.Order), typeof(int));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseMediaToProductRelationshipDto releaseMediaToProductRelationship in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseMediaToProductRelationships))
        {
            releaseMediaToProductRelationshipsDataTable.Rows.Add(
                releaseMediaToProductRelationship.MediaNumber.AsDbValue(),
                releaseMediaToProductRelationship.ReleaseId.AsDbValue(),
                releaseMediaToProductRelationship.ProductId.AsDbValue(),
                releaseMediaToProductRelationship.Name.AsDbValue(),
                releaseMediaToProductRelationship.Description.AsDbValue(),
                releaseMediaToProductRelationship.Order.AsDbValue(),
                releaseMediaToProductRelationship.ReferenceOrder.AsDbValue());
        }

        using var releaseTrackCollectionDataTable = new DataTable();
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.TrackNumber), typeof(byte));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.MediaNumber), typeof(byte));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.ReleaseId), typeof(Guid));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.Title), typeof(string));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.Description), typeof(string));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.DisambiguationText), typeof(string));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.InternationalStandardRecordingCode), typeof(string));
        foreach (ReleaseTrackDto releaseTrack in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection))
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

        using var releaseTrackToProductRelationshipsDataTable = new DataTable();
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.TrackNumber), typeof(byte));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.MediaNumber), typeof(byte));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.ReleaseId), typeof(Guid));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.ProductId), typeof(Guid));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.Name), typeof(string));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.Description), typeof(string));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.Order), typeof(int));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseTrackToProductRelationshipDto releaseTrackToProductRelationship in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackToProductRelationships))
        {
            releaseTrackToProductRelationshipsDataTable.Rows.Add(
                releaseTrackToProductRelationship.TrackNumber.AsDbValue(),
                releaseTrackToProductRelationship.MediaNumber.AsDbValue(),
                releaseTrackToProductRelationship.ReleaseId.AsDbValue(),
                releaseTrackToProductRelationship.ProductId.AsDbValue(),
                releaseTrackToProductRelationship.Name.AsDbValue(),
                releaseTrackToProductRelationship.Description.AsDbValue(),
                releaseTrackToProductRelationship.Order.AsDbValue(),
                releaseTrackToProductRelationship.ReferenceOrder.AsDbValue());
        }

        using var releaseTrackToWorkRelationshipsDataTable = new DataTable();
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.TrackNumber), typeof(byte));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.MediaNumber), typeof(byte));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.ReleaseId), typeof(Guid));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.WorkId), typeof(Guid));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.Name), typeof(string));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.Description), typeof(string));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.Order), typeof(int));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseTrackToWorkRelationshipDto releaseTrackToWorkRelationship in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackToWorkRelationships))
        {
            releaseTrackToWorkRelationshipsDataTable.Rows.Add(
                releaseTrackToWorkRelationship.TrackNumber.AsDbValue(),
                releaseTrackToWorkRelationship.MediaNumber.AsDbValue(),
                releaseTrackToWorkRelationship.ReleaseId.AsDbValue(),
                releaseTrackToWorkRelationship.WorkId.AsDbValue(),
                releaseTrackToWorkRelationship.Name.AsDbValue(),
                releaseTrackToWorkRelationship.Description.AsDbValue(),
                releaseTrackToWorkRelationship.Order.AsDbValue(),
                releaseTrackToWorkRelationship.ReferenceOrder.AsDbValue());
        }

        using var releaseTrackArtistsDataTable = new DataTable();
        releaseTrackArtistsDataTable.Columns.Add(nameof(ReleaseTrackArtistDto.TrackNumber), typeof(byte));
        releaseTrackArtistsDataTable.Columns.Add(nameof(ReleaseTrackArtistDto.MediaNumber), typeof(byte));
        releaseTrackArtistsDataTable.Columns.Add(nameof(ReleaseTrackArtistDto.ReleaseId), typeof(Guid));
        releaseTrackArtistsDataTable.Columns.Add(nameof(ReleaseTrackArtistDto.ArtistId), typeof(Guid));
        releaseTrackArtistsDataTable.Columns.Add(nameof(ReleaseTrackArtistDto.Order), typeof(int));
        foreach (ReleaseTrackArtistDto releaseTrackArtist in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackArtists))
        {
            releaseTrackArtistsDataTable.Rows.Add(
                releaseTrackArtist.TrackNumber.AsDbValue(),
                releaseTrackArtist.MediaNumber.AsDbValue(),
                releaseTrackArtist.ReleaseId.AsDbValue(),
                releaseTrackArtist.ArtistId.AsDbValue(),
                releaseTrackArtist.Order.AsDbValue());
        }

        using var releaseTrackFeaturedArtistsDataTable = new DataTable();
        releaseTrackFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseTrackFeaturedArtistDto.TrackNumber), typeof(byte));
        releaseTrackFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseTrackFeaturedArtistDto.MediaNumber), typeof(byte));
        releaseTrackFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseTrackFeaturedArtistDto.ReleaseId), typeof(Guid));
        releaseTrackFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseTrackFeaturedArtistDto.ArtistId), typeof(Guid));
        releaseTrackFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseTrackFeaturedArtistDto.Order), typeof(int));
        foreach (ReleaseTrackFeaturedArtistDto releaseTrackFeaturedArtist in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackFeaturedArtists))
        {
            releaseTrackFeaturedArtistsDataTable.Rows.Add(
                releaseTrackFeaturedArtist.TrackNumber.AsDbValue(),
                releaseTrackFeaturedArtist.MediaNumber.AsDbValue(),
                releaseTrackFeaturedArtist.ReleaseId.AsDbValue(),
                releaseTrackFeaturedArtist.ArtistId.AsDbValue(),
                releaseTrackFeaturedArtist.Order.AsDbValue());
        }

        using var releaseTrackPerformersDataTable = new DataTable();
        releaseTrackPerformersDataTable.Columns.Add(nameof(ReleaseTrackPerformerDto.TrackNumber), typeof(byte));
        releaseTrackPerformersDataTable.Columns.Add(nameof(ReleaseTrackPerformerDto.MediaNumber), typeof(byte));
        releaseTrackPerformersDataTable.Columns.Add(nameof(ReleaseTrackPerformerDto.ReleaseId), typeof(Guid));
        releaseTrackPerformersDataTable.Columns.Add(nameof(ReleaseTrackPerformerDto.ArtistId), typeof(Guid));
        releaseTrackPerformersDataTable.Columns.Add(nameof(ReleaseTrackPerformerDto.Order), typeof(int));
        foreach (ReleaseTrackPerformerDto releaseTrackPerformer in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackPerformers))
        {
            releaseTrackPerformersDataTable.Rows.Add(
                releaseTrackPerformer.TrackNumber.AsDbValue(),
                releaseTrackPerformer.MediaNumber.AsDbValue(),
                releaseTrackPerformer.ReleaseId.AsDbValue(),
                releaseTrackPerformer.ArtistId.AsDbValue(),
                releaseTrackPerformer.Order.AsDbValue());
        }

        using var releaseTrackComposersDataTable = new DataTable();
        releaseTrackComposersDataTable.Columns.Add(nameof(ReleaseTrackComposerDto.TrackNumber), typeof(byte));
        releaseTrackComposersDataTable.Columns.Add(nameof(ReleaseTrackComposerDto.MediaNumber), typeof(byte));
        releaseTrackComposersDataTable.Columns.Add(nameof(ReleaseTrackComposerDto.ReleaseId), typeof(Guid));
        releaseTrackComposersDataTable.Columns.Add(nameof(ReleaseTrackComposerDto.ArtistId), typeof(Guid));
        releaseTrackComposersDataTable.Columns.Add(nameof(ReleaseTrackComposerDto.Order), typeof(int));
        foreach (ReleaseTrackComposerDto releaseTrackComposer in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackComposers))
        {
            releaseTrackComposersDataTable.Rows.Add(
                releaseTrackComposer.TrackNumber.AsDbValue(),
                releaseTrackComposer.MediaNumber.AsDbValue(),
                releaseTrackComposer.ReleaseId.AsDbValue(),
                releaseTrackComposer.ArtistId.AsDbValue(),
                releaseTrackComposer.Order.AsDbValue());
        }

        using var releaseTrackGenresDataTable = new DataTable();
        releaseTrackGenresDataTable.Columns.Add(nameof(ReleaseTrackGenreDto.TrackNumber), typeof(byte));
        releaseTrackGenresDataTable.Columns.Add(nameof(ReleaseTrackGenreDto.MediaNumber), typeof(byte));
        releaseTrackGenresDataTable.Columns.Add(nameof(ReleaseTrackGenreDto.ReleaseId), typeof(Guid));
        releaseTrackGenresDataTable.Columns.Add(nameof(ReleaseTrackGenreDto.GenreId), typeof(Guid));
        releaseTrackGenresDataTable.Columns.Add(nameof(ReleaseTrackGenreDto.Order), typeof(int));
        foreach (ReleaseTrackGenreDto releaseTrackGenre in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackGenres))
        {
            releaseTrackGenresDataTable.Rows.Add(
                releaseTrackGenre.TrackNumber.AsDbValue(),
                releaseTrackGenre.MediaNumber.AsDbValue(),
                releaseTrackGenre.ReleaseId.AsDbValue(),
                releaseTrackGenre.GenreId.AsDbValue(),
                releaseTrackGenre.Order.AsDbValue());
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
            new SqlParameter(nameof(ReleaseDto.MediaFormat), release.MediaFormat.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.PublishFormat), release.PublishFormat.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.CatalogNumber), release.CatalogNumber.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Barcode), release.Barcode.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.ReleasedOn), release.ReleasedOn.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.ReleasedOnYearOnly), release.ReleasedOnYearOnly.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Enabled), release.Enabled.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.ReleaseRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseRelationship]", Value = releaseRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseToProductRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseToProductRelationship]", Value = releaseToProductRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseToReleaseGroupRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseToReleaseGroupRelationship]", Value = releaseToReleaseGroupRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseArtists), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseArtist]", Value = releaseArtistsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseFeaturedArtists), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseFeaturedArtist]", Value = releaseFeaturedArtistsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleasePerformers), SqlDbType.Structured) { TypeName = "[dbo].[ReleasePerformer]", Value = releasePerformersDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseComposers), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseComposer]", Value = releaseComposersDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseGenres), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseGenre]", Value = releaseGenresDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseMediaCollection), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseMedia]", Value = releaseMediaCollectionDataTable },
            new SqlParameter(nameof(ReleaseMediaDto.ReleaseMediaToProductRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseMediaToProductRelationship]", Value = releaseMediaToProductRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseMediaDto.ReleaseTrackCollection), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrack]", Value = releaseTrackCollectionDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackToProductRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackToProductRelationship]", Value = releaseTrackToProductRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackToWorkRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackToWorkRelationship]", Value = releaseTrackToWorkRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackArtists), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackArtist]", Value = releaseTrackArtistsDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackFeaturedArtists), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackFeaturedArtist]", Value = releaseTrackFeaturedArtistsDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackPerformers), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackPerformer]", Value = releaseTrackPerformersDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackComposers), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackComposer]", Value = releaseTrackComposersDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackGenres), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackGenre]", Value = releaseTrackGenresDataTable },
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
                @{nameof(ReleaseDto.MediaFormat)},
                @{nameof(ReleaseDto.PublishFormat)},
                @{nameof(ReleaseDto.CatalogNumber)},
                @{nameof(ReleaseDto.Barcode)},
                @{nameof(ReleaseDto.ReleasedOn)},
                @{nameof(ReleaseDto.ReleasedOnYearOnly)},
                @{nameof(ReleaseDto.Enabled)},
                @{nameof(ReleaseDto.ReleaseRelationships)},
                @{nameof(ReleaseDto.ReleaseToProductRelationships)},
                @{nameof(ReleaseDto.ReleaseToReleaseGroupRelationships)},
                @{nameof(ReleaseDto.ReleaseArtists)},
                @{nameof(ReleaseDto.ReleaseFeaturedArtists)},
                @{nameof(ReleaseDto.ReleasePerformers)},
                @{nameof(ReleaseDto.ReleaseComposers)},
                @{nameof(ReleaseDto.ReleaseGenres)},
                @{nameof(ReleaseDto.ReleaseMediaCollection)},
                @{nameof(ReleaseMediaDto.ReleaseMediaToProductRelationships)},
                @{nameof(ReleaseMediaDto.ReleaseTrackCollection)},
                @{nameof(ReleaseTrackDto.ReleaseTrackToProductRelationships)},
                @{nameof(ReleaseTrackDto.ReleaseTrackToWorkRelationships)},
                @{nameof(ReleaseTrackDto.ReleaseTrackArtists)},
                @{nameof(ReleaseTrackDto.ReleaseTrackFeaturedArtists)},
                @{nameof(ReleaseTrackDto.ReleaseTrackPerformers)},
                @{nameof(ReleaseTrackDto.ReleaseTrackComposers)},
                @{nameof(ReleaseTrackDto.ReleaseTrackGenres)},
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
        SetReleaseToProductRelationshipOrders(release.ReleaseToProductRelationships);
        SetReleaseToReleaseGroupRelationshipOrders(release.ReleaseToReleaseGroupRelationships);
        SetReleaseArtistOrders(release.ReleaseArtists);
        SetReleaseFeaturedArtistOrders(release.ReleaseFeaturedArtists);
        SetReleasePerformerOrders(release.ReleasePerformers);
        SetReleaseComposerOrders(release.ReleaseComposers);
        SetReleaseGenreOrders(release.ReleaseGenres);

        SetReleaseMediaForeignKeys(release.Id, release.ReleaseMediaCollection);
        foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
        {
            SetReleaseMediaToProductRelationshipOrders(releaseMedia.MediaNumber, releaseMedia.ReleaseId, releaseMedia.ReleaseMediaToProductRelationships);

            SetReleaseTrackForeignKeys(releaseMedia.MediaNumber, releaseMedia.ReleaseId, releaseMedia.ReleaseTrackCollection);
            foreach (ReleaseTrackDto releaseTrack in releaseMedia.ReleaseTrackCollection)
            {
                SetReleaseTrackToProductRelationshipOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackToProductRelationships);
                SetReleaseTrackToWorkRelationshipOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackToWorkRelationships);
                SetReleaseTrackArtistOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackArtists);
                SetReleaseTrackFeaturedArtistOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackFeaturedArtists);
                SetReleaseTrackPerformerOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackPerformers);
                SetReleaseTrackComposerOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackComposers);
                SetReleaseTrackGenreOrders(releaseTrack.TrackNumber, releaseTrack.MediaNumber, releaseTrack.ReleaseId, releaseTrack.ReleaseTrackGenres);
            }
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

        using var releaseToProductRelationshipsDataTable = new DataTable();
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.ReleaseId), typeof(Guid));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.ProductId), typeof(Guid));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.Name), typeof(string));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.Description), typeof(string));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.Order), typeof(int));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseToProductRelationshipDto releaseToProductRelationship in release.ReleaseToProductRelationships)
        {
            releaseToProductRelationshipsDataTable.Rows.Add(
                releaseToProductRelationship.ReleaseId.AsDbValue(),
                releaseToProductRelationship.ProductId.AsDbValue(),
                releaseToProductRelationship.Name.AsDbValue(),
                releaseToProductRelationship.Description.AsDbValue(),
                releaseToProductRelationship.Order.AsDbValue(),
                releaseToProductRelationship.ReferenceOrder.AsDbValue());
        }

        using var releaseToReleaseGroupRelationshipsDataTable = new DataTable();
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.ReleaseId), typeof(Guid));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.ReleaseGroupId), typeof(Guid));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.Name), typeof(string));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.Description), typeof(string));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.Order), typeof(int));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseToReleaseGroupRelationshipDto releaseToReleaseGroupRelationship in release.ReleaseToReleaseGroupRelationships)
        {
            releaseToReleaseGroupRelationshipsDataTable.Rows.Add(
                releaseToReleaseGroupRelationship.ReleaseId.AsDbValue(),
                releaseToReleaseGroupRelationship.ReleaseGroupId.AsDbValue(),
                releaseToReleaseGroupRelationship.Name.AsDbValue(),
                releaseToReleaseGroupRelationship.Description.AsDbValue(),
                releaseToReleaseGroupRelationship.Order.AsDbValue(),
                releaseToReleaseGroupRelationship.ReferenceOrder.AsDbValue());
        }

        using var releaseArtistsDataTable = new DataTable();
        releaseArtistsDataTable.Columns.Add(nameof(ReleaseArtistDto.ReleaseId), typeof(Guid));
        releaseArtistsDataTable.Columns.Add(nameof(ReleaseArtistDto.ArtistId), typeof(Guid));
        releaseArtistsDataTable.Columns.Add(nameof(ReleaseArtistDto.Order), typeof(int));
        foreach (ReleaseArtistDto releaseArtist in release.ReleaseArtists)
        {
            releaseArtistsDataTable.Rows.Add(
                releaseArtist.ReleaseId.AsDbValue(),
                releaseArtist.ArtistId.AsDbValue(),
                releaseArtist.Order.AsDbValue());
        }

        using var releaseFeaturedArtistsDataTable = new DataTable();
        releaseFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseFeaturedArtistDto.ReleaseId), typeof(Guid));
        releaseFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseFeaturedArtistDto.ArtistId), typeof(Guid));
        releaseFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseFeaturedArtistDto.Order), typeof(int));
        foreach (ReleaseFeaturedArtistDto releaseFeaturedArtist in release.ReleaseFeaturedArtists)
        {
            releaseFeaturedArtistsDataTable.Rows.Add(
                releaseFeaturedArtist.ReleaseId.AsDbValue(),
                releaseFeaturedArtist.ArtistId.AsDbValue(),
                releaseFeaturedArtist.Order.AsDbValue());
        }

        using var releasePerformersDataTable = new DataTable();
        releasePerformersDataTable.Columns.Add(nameof(ReleasePerformerDto.ReleaseId), typeof(Guid));
        releasePerformersDataTable.Columns.Add(nameof(ReleasePerformerDto.ArtistId), typeof(Guid));
        releasePerformersDataTable.Columns.Add(nameof(ReleasePerformerDto.Order), typeof(int));
        foreach (ReleasePerformerDto releasePerformer in release.ReleasePerformers)
        {
            releasePerformersDataTable.Rows.Add(
                releasePerformer.ReleaseId.AsDbValue(),
                releasePerformer.ArtistId.AsDbValue(),
                releasePerformer.Order.AsDbValue());
        }

        using var releaseComposersDataTable = new DataTable();
        releaseComposersDataTable.Columns.Add(nameof(ReleaseComposerDto.ReleaseId), typeof(Guid));
        releaseComposersDataTable.Columns.Add(nameof(ReleaseComposerDto.ArtistId), typeof(Guid));
        releaseComposersDataTable.Columns.Add(nameof(ReleaseComposerDto.Order), typeof(int));
        foreach (ReleaseComposerDto releaseComposer in release.ReleaseComposers)
        {
            releaseComposersDataTable.Rows.Add(
                releaseComposer.ReleaseId.AsDbValue(),
                releaseComposer.ArtistId.AsDbValue(),
                releaseComposer.Order.AsDbValue());
        }

        using var releaseGenresDataTable = new DataTable();
        releaseGenresDataTable.Columns.Add(nameof(ReleaseGenreDto.ReleaseId), typeof(Guid));
        releaseGenresDataTable.Columns.Add(nameof(ReleaseGenreDto.GenreId), typeof(Guid));
        releaseGenresDataTable.Columns.Add(nameof(ReleaseGenreDto.Order), typeof(int));
        foreach (ReleaseGenreDto releaseGenre in release.ReleaseGenres)
        {
            releaseGenresDataTable.Rows.Add(
                releaseGenre.ReleaseId.AsDbValue(),
                releaseGenre.GenreId.AsDbValue(),
                releaseGenre.Order.AsDbValue());
        }

        using var releaseMediaCollectionDataTable = new DataTable();
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.MediaNumber), typeof(byte));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.ReleaseId), typeof(Guid));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.Title), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.Description), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.DisambiguationText), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.MediaFormat), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.CatalogNumber), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.FreeDbChecksum), typeof(string));
        releaseMediaCollectionDataTable.Columns.Add(nameof(ReleaseMediaDto.MusicBrainzChecksum), typeof(string));
        foreach (ReleaseMediaDto releaseMedia in release.ReleaseMediaCollection)
        {
            releaseMediaCollectionDataTable.Rows.Add(
                releaseMedia.MediaNumber.AsDbValue(),
                releaseMedia.ReleaseId.AsDbValue(),
                releaseMedia.Title.AsDbValue(),
                releaseMedia.Description.AsDbValue(),
                releaseMedia.DisambiguationText.AsDbValue(),
                releaseMedia.MediaFormat.AsDbValue(),
                releaseMedia.CatalogNumber.AsDbValue(),
                releaseMedia.FreeDbChecksum.AsDbValue(),
                releaseMedia.MusicBrainzChecksum.AsDbValue());
        }

        using var releaseMediaToProductRelationshipsDataTable = new DataTable();
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.MediaNumber), typeof(byte));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.ReleaseId), typeof(Guid));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.ProductId), typeof(Guid));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.Name), typeof(string));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.Description), typeof(string));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.Order), typeof(int));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseMediaToProductRelationshipDto releaseMediaToProductRelationship in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseMediaToProductRelationships))
        {
            releaseMediaToProductRelationshipsDataTable.Rows.Add(
                releaseMediaToProductRelationship.MediaNumber.AsDbValue(),
                releaseMediaToProductRelationship.ReleaseId.AsDbValue(),
                releaseMediaToProductRelationship.ProductId.AsDbValue(),
                releaseMediaToProductRelationship.Name.AsDbValue(),
                releaseMediaToProductRelationship.Description.AsDbValue(),
                releaseMediaToProductRelationship.Order.AsDbValue(),
                releaseMediaToProductRelationship.ReferenceOrder.AsDbValue());
        }

        using var releaseTrackCollectionDataTable = new DataTable();
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.TrackNumber), typeof(byte));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.MediaNumber), typeof(byte));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.ReleaseId), typeof(Guid));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.Title), typeof(string));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.Description), typeof(string));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.DisambiguationText), typeof(string));
        releaseTrackCollectionDataTable.Columns.Add(nameof(ReleaseTrackDto.InternationalStandardRecordingCode), typeof(string));
        foreach (ReleaseTrackDto releaseTrack in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection))
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

        using var releaseTrackToProductRelationshipsDataTable = new DataTable();
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.TrackNumber), typeof(byte));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.MediaNumber), typeof(byte));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.ReleaseId), typeof(Guid));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.ProductId), typeof(Guid));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.Name), typeof(string));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.Description), typeof(string));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.Order), typeof(int));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseTrackToProductRelationshipDto releaseTrackToProductRelationship in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackToProductRelationships))
        {
            releaseTrackToProductRelationshipsDataTable.Rows.Add(
                releaseTrackToProductRelationship.TrackNumber.AsDbValue(),
                releaseTrackToProductRelationship.MediaNumber.AsDbValue(),
                releaseTrackToProductRelationship.ReleaseId.AsDbValue(),
                releaseTrackToProductRelationship.ProductId.AsDbValue(),
                releaseTrackToProductRelationship.Name.AsDbValue(),
                releaseTrackToProductRelationship.Description.AsDbValue(),
                releaseTrackToProductRelationship.Order.AsDbValue(),
                releaseTrackToProductRelationship.ReferenceOrder.AsDbValue());
        }

        using var releaseTrackToWorkRelationshipsDataTable = new DataTable();
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.TrackNumber), typeof(byte));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.MediaNumber), typeof(byte));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.ReleaseId), typeof(Guid));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.WorkId), typeof(Guid));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.Name), typeof(string));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.Description), typeof(string));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.Order), typeof(int));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseTrackToWorkRelationshipDto releaseTrackToWorkRelationship in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackToWorkRelationships))
        {
            releaseTrackToWorkRelationshipsDataTable.Rows.Add(
                releaseTrackToWorkRelationship.TrackNumber.AsDbValue(),
                releaseTrackToWorkRelationship.MediaNumber.AsDbValue(),
                releaseTrackToWorkRelationship.ReleaseId.AsDbValue(),
                releaseTrackToWorkRelationship.WorkId.AsDbValue(),
                releaseTrackToWorkRelationship.Name.AsDbValue(),
                releaseTrackToWorkRelationship.Description.AsDbValue(),
                releaseTrackToWorkRelationship.Order.AsDbValue(),
                releaseTrackToWorkRelationship.ReferenceOrder.AsDbValue());
        }

        using var releaseTrackArtistsDataTable = new DataTable();
        releaseTrackArtistsDataTable.Columns.Add(nameof(ReleaseTrackArtistDto.TrackNumber), typeof(byte));
        releaseTrackArtistsDataTable.Columns.Add(nameof(ReleaseTrackArtistDto.MediaNumber), typeof(byte));
        releaseTrackArtistsDataTable.Columns.Add(nameof(ReleaseTrackArtistDto.ReleaseId), typeof(Guid));
        releaseTrackArtistsDataTable.Columns.Add(nameof(ReleaseTrackArtistDto.ArtistId), typeof(Guid));
        releaseTrackArtistsDataTable.Columns.Add(nameof(ReleaseTrackArtistDto.Order), typeof(int));
        foreach (ReleaseTrackArtistDto releaseTrackArtist in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackArtists))
        {
            releaseTrackArtistsDataTable.Rows.Add(
                releaseTrackArtist.TrackNumber.AsDbValue(),
                releaseTrackArtist.MediaNumber.AsDbValue(),
                releaseTrackArtist.ReleaseId.AsDbValue(),
                releaseTrackArtist.ArtistId.AsDbValue(),
                releaseTrackArtist.Order.AsDbValue());
        }

        using var releaseTrackFeaturedArtistsDataTable = new DataTable();
        releaseTrackFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseTrackFeaturedArtistDto.TrackNumber), typeof(byte));
        releaseTrackFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseTrackFeaturedArtistDto.MediaNumber), typeof(byte));
        releaseTrackFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseTrackFeaturedArtistDto.ReleaseId), typeof(Guid));
        releaseTrackFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseTrackFeaturedArtistDto.ArtistId), typeof(Guid));
        releaseTrackFeaturedArtistsDataTable.Columns.Add(nameof(ReleaseTrackFeaturedArtistDto.Order), typeof(int));
        foreach (ReleaseTrackFeaturedArtistDto releaseTrackFeaturedArtist in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackFeaturedArtists))
        {
            releaseTrackFeaturedArtistsDataTable.Rows.Add(
                releaseTrackFeaturedArtist.TrackNumber.AsDbValue(),
                releaseTrackFeaturedArtist.MediaNumber.AsDbValue(),
                releaseTrackFeaturedArtist.ReleaseId.AsDbValue(),
                releaseTrackFeaturedArtist.ArtistId.AsDbValue(),
                releaseTrackFeaturedArtist.Order.AsDbValue());
        }

        using var releaseTrackPerformersDataTable = new DataTable();
        releaseTrackPerformersDataTable.Columns.Add(nameof(ReleaseTrackPerformerDto.TrackNumber), typeof(byte));
        releaseTrackPerformersDataTable.Columns.Add(nameof(ReleaseTrackPerformerDto.MediaNumber), typeof(byte));
        releaseTrackPerformersDataTable.Columns.Add(nameof(ReleaseTrackPerformerDto.ReleaseId), typeof(Guid));
        releaseTrackPerformersDataTable.Columns.Add(nameof(ReleaseTrackPerformerDto.ArtistId), typeof(Guid));
        releaseTrackPerformersDataTable.Columns.Add(nameof(ReleaseTrackPerformerDto.Order), typeof(int));
        foreach (ReleaseTrackPerformerDto releaseTrackPerformer in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackPerformers))
        {
            releaseTrackPerformersDataTable.Rows.Add(
                releaseTrackPerformer.TrackNumber.AsDbValue(),
                releaseTrackPerformer.MediaNumber.AsDbValue(),
                releaseTrackPerformer.ReleaseId.AsDbValue(),
                releaseTrackPerformer.ArtistId.AsDbValue(),
                releaseTrackPerformer.Order.AsDbValue());
        }

        using var releaseTrackComposersDataTable = new DataTable();
        releaseTrackComposersDataTable.Columns.Add(nameof(ReleaseTrackComposerDto.TrackNumber), typeof(byte));
        releaseTrackComposersDataTable.Columns.Add(nameof(ReleaseTrackComposerDto.MediaNumber), typeof(byte));
        releaseTrackComposersDataTable.Columns.Add(nameof(ReleaseTrackComposerDto.ReleaseId), typeof(Guid));
        releaseTrackComposersDataTable.Columns.Add(nameof(ReleaseTrackComposerDto.ArtistId), typeof(Guid));
        releaseTrackComposersDataTable.Columns.Add(nameof(ReleaseTrackComposerDto.Order), typeof(int));
        foreach (ReleaseTrackComposerDto releaseTrackComposer in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackComposers))
        {
            releaseTrackComposersDataTable.Rows.Add(
                releaseTrackComposer.TrackNumber.AsDbValue(),
                releaseTrackComposer.MediaNumber.AsDbValue(),
                releaseTrackComposer.ReleaseId.AsDbValue(),
                releaseTrackComposer.ArtistId.AsDbValue(),
                releaseTrackComposer.Order.AsDbValue());
        }

        using var releaseTrackGenresDataTable = new DataTable();
        releaseTrackGenresDataTable.Columns.Add(nameof(ReleaseTrackGenreDto.TrackNumber), typeof(byte));
        releaseTrackGenresDataTable.Columns.Add(nameof(ReleaseTrackGenreDto.MediaNumber), typeof(byte));
        releaseTrackGenresDataTable.Columns.Add(nameof(ReleaseTrackGenreDto.ReleaseId), typeof(Guid));
        releaseTrackGenresDataTable.Columns.Add(nameof(ReleaseTrackGenreDto.GenreId), typeof(Guid));
        releaseTrackGenresDataTable.Columns.Add(nameof(ReleaseTrackGenreDto.Order), typeof(int));
        foreach (ReleaseTrackGenreDto releaseTrackGenre in release.ReleaseMediaCollection
            .SelectMany(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .SelectMany(releaseTrack => releaseTrack.ReleaseTrackGenres))
        {
            releaseTrackGenresDataTable.Rows.Add(
                releaseTrackGenre.TrackNumber.AsDbValue(),
                releaseTrackGenre.MediaNumber.AsDbValue(),
                releaseTrackGenre.ReleaseId.AsDbValue(),
                releaseTrackGenre.GenreId.AsDbValue(),
                releaseTrackGenre.Order.AsDbValue());
        }

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ReleaseDto.Id), release.Id.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Title), release.Title.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Description), release.Description.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.DisambiguationText), release.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.MediaFormat), release.MediaFormat.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.PublishFormat), release.PublishFormat.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.CatalogNumber), release.CatalogNumber.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Barcode), release.Barcode.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.ReleasedOn), release.ReleasedOn.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.ReleasedOnYearOnly), release.ReleasedOnYearOnly.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.Enabled), release.Enabled.AsDbValue()),
            new SqlParameter(nameof(ReleaseDto.ReleaseRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseRelationship]", Value = releaseRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseToProductRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseToProductRelationship]", Value = releaseToProductRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseToReleaseGroupRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseToReleaseGroupRelationship]", Value = releaseToReleaseGroupRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseArtists), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseArtist]", Value = releaseArtistsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseFeaturedArtists), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseFeaturedArtist]", Value = releaseFeaturedArtistsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleasePerformers), SqlDbType.Structured) { TypeName = "[dbo].[ReleasePerformer]", Value = releasePerformersDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseComposers), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseComposer]", Value = releaseComposersDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseGenres), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseGenre]", Value = releaseGenresDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseMediaCollection), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseMedia]", Value = releaseMediaCollectionDataTable },
            new SqlParameter(nameof(ReleaseMediaDto.ReleaseMediaToProductRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseMediaToProductRelationship]", Value = releaseMediaToProductRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseMediaDto.ReleaseTrackCollection), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrack]", Value = releaseTrackCollectionDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackToProductRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackToProductRelationship]", Value = releaseTrackToProductRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackToWorkRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackToWorkRelationship]", Value = releaseTrackToWorkRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackArtists), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackArtist]", Value = releaseTrackArtistsDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackFeaturedArtists), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackFeaturedArtist]", Value = releaseTrackFeaturedArtistsDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackPerformers), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackPerformer]", Value = releaseTrackPerformersDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackComposers), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackComposer]", Value = releaseTrackComposersDataTable },
            new SqlParameter(nameof(ReleaseTrackDto.ReleaseTrackGenres), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackGenre]", Value = releaseTrackGenresDataTable },
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateRelease]
                @{nameof(ReleaseDto.Id)},
                @{nameof(ReleaseDto.Title)},
                @{nameof(ReleaseDto.Description)},
                @{nameof(ReleaseDto.DisambiguationText)},
                @{nameof(ReleaseDto.MediaFormat)},
                @{nameof(ReleaseDto.PublishFormat)},
                @{nameof(ReleaseDto.CatalogNumber)},
                @{nameof(ReleaseDto.Barcode)},
                @{nameof(ReleaseDto.ReleasedOn)},
                @{nameof(ReleaseDto.ReleasedOnYearOnly)},
                @{nameof(ReleaseDto.Enabled)},
                @{nameof(ReleaseDto.ReleaseRelationships)},
                @{nameof(ReleaseDto.ReleaseToProductRelationships)},
                @{nameof(ReleaseDto.ReleaseToReleaseGroupRelationships)},
                @{nameof(ReleaseDto.ReleaseArtists)},
                @{nameof(ReleaseDto.ReleaseFeaturedArtists)},
                @{nameof(ReleaseDto.ReleasePerformers)},
                @{nameof(ReleaseDto.ReleaseComposers)},
                @{nameof(ReleaseDto.ReleaseGenres)},
                @{nameof(ReleaseDto.ReleaseMediaCollection)},
                @{nameof(ReleaseMediaDto.ReleaseMediaToProductRelationships)},
                @{nameof(ReleaseMediaDto.ReleaseTrackCollection)},
                @{nameof(ReleaseTrackDto.ReleaseTrackToProductRelationships)},
                @{nameof(ReleaseTrackDto.ReleaseTrackToWorkRelationships)},
                @{nameof(ReleaseTrackDto.ReleaseTrackArtists)},
                @{nameof(ReleaseTrackDto.ReleaseTrackFeaturedArtists)},
                @{nameof(ReleaseTrackDto.ReleaseTrackPerformers)},
                @{nameof(ReleaseTrackDto.ReleaseTrackComposers)},
                @{nameof(ReleaseTrackDto.ReleaseTrackGenres)},
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated > 0;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseToProductRelationshipsOrderAsync(ReleaseToProductRelationshipDto[] releaseToProductRelationships, bool useReferenceOrder)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var i = 0;
        foreach (ReleaseToProductRelationshipDto releaseToProductRelationship in releaseToProductRelationships)
        {
            if (!useReferenceOrder)
                releaseToProductRelationship.Order = i++;
            else
                releaseToProductRelationship.ReferenceOrder = i++;
        }

        using var releaseToProductRelationshipsDataTable = new DataTable();
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.ReleaseId), typeof(Guid));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.ProductId), typeof(Guid));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.Name), typeof(string));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.Description), typeof(string));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.Order), typeof(int));
        releaseToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseToProductRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseToProductRelationshipDto releaseToProductRelationship in releaseToProductRelationships)
        {
            releaseToProductRelationshipsDataTable.Rows.Add(
                releaseToProductRelationship.ReleaseId.AsDbValue(),
                releaseToProductRelationship.ProductId.AsDbValue(),
                releaseToProductRelationship.Name.AsDbValue(),
                releaseToProductRelationship.Description.AsDbValue(),
                releaseToProductRelationship.Order.AsDbValue(),
                releaseToProductRelationship.ReferenceOrder.AsDbValue());
        }

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter("UseReferenceOrder", useReferenceOrder.AsDbValue()),
            new SqlParameter("ReleaseToProductRelationships", SqlDbType.Structured) { TypeName = "[dbo].[ReleaseToProductRelationship]", Value = releaseToProductRelationshipsDataTable },
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateReleaseToProductRelationshipsOrder]
                @UseReferenceOrder,
                @ReleaseToProductRelationships,
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated >= 0;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseToReleaseGroupRelationshipsOrderAsync(ReleaseToReleaseGroupRelationshipDto[] releaseToReleaseGroupRelationships, bool useReferenceOrder)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var i = 0;
        foreach (ReleaseToReleaseGroupRelationshipDto releaseToReleaseGroupRelationship in releaseToReleaseGroupRelationships)
        {
            if (!useReferenceOrder)
                releaseToReleaseGroupRelationship.Order = i++;
            else
                releaseToReleaseGroupRelationship.ReferenceOrder = i++;
        }

        using var releaseToReleaseGroupRelationshipsDataTable = new DataTable();
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.ReleaseId), typeof(Guid));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.ReleaseGroupId), typeof(Guid));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.Name), typeof(string));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.Description), typeof(string));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.Order), typeof(int));
        releaseToReleaseGroupRelationshipsDataTable.Columns.Add(nameof(ReleaseToReleaseGroupRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseToReleaseGroupRelationshipDto releaseToReleaseGroupRelationship in releaseToReleaseGroupRelationships)
        {
            releaseToReleaseGroupRelationshipsDataTable.Rows.Add(
                releaseToReleaseGroupRelationship.ReleaseId.AsDbValue(),
                releaseToReleaseGroupRelationship.ReleaseGroupId.AsDbValue(),
                releaseToReleaseGroupRelationship.Name.AsDbValue(),
                releaseToReleaseGroupRelationship.Description.AsDbValue(),
                releaseToReleaseGroupRelationship.Order.AsDbValue(),
                releaseToReleaseGroupRelationship.ReferenceOrder.AsDbValue());
        }

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter("UseReferenceOrder", useReferenceOrder.AsDbValue()),
            new SqlParameter("ReleaseToReleaseGroupRelationships", SqlDbType.Structured) { TypeName = "[dbo].[ReleaseToReleaseGroupRelationship]", Value = releaseToReleaseGroupRelationshipsDataTable },
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateReleaseToReleaseGroupRelationshipsOrder]
                @UseReferenceOrder,
                @ReleaseToReleaseGroupRelationships,
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated >= 0;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseMediaToProductRelationshipsOrderAsync(ReleaseMediaToProductRelationshipDto[] releaseMediaToProductRelationships, bool useReferenceOrder)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var i = 0;
        foreach (ReleaseMediaToProductRelationshipDto releaseMediaToProductRelationship in releaseMediaToProductRelationships)
        {
            if (!useReferenceOrder)
                releaseMediaToProductRelationship.Order = i++;
            else
                releaseMediaToProductRelationship.ReferenceOrder = i++;
        }

        using var releaseMediaToProductRelationshipsDataTable = new DataTable();
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.MediaNumber), typeof(byte));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.ReleaseId), typeof(Guid));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.ProductId), typeof(Guid));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.Name), typeof(string));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.Description), typeof(string));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.Order), typeof(int));
        releaseMediaToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseMediaToProductRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseMediaToProductRelationshipDto releaseMediaToProductRelationship in releaseMediaToProductRelationships)
        {
            releaseMediaToProductRelationshipsDataTable.Rows.Add(
                releaseMediaToProductRelationship.MediaNumber.AsDbValue(),
                releaseMediaToProductRelationship.ReleaseId.AsDbValue(),
                releaseMediaToProductRelationship.ProductId.AsDbValue(),
                releaseMediaToProductRelationship.Name.AsDbValue(),
                releaseMediaToProductRelationship.Description.AsDbValue(),
                releaseMediaToProductRelationship.Order.AsDbValue(),
                releaseMediaToProductRelationship.ReferenceOrder.AsDbValue());
        }

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter("UseReferenceOrder", useReferenceOrder.AsDbValue()),
            new SqlParameter("ReleaseMediaToProductRelationships", SqlDbType.Structured) { TypeName = "[dbo].[ReleaseMediaToProductRelationship]", Value = releaseMediaToProductRelationshipsDataTable },
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateReleaseMediaToProductRelationshipsOrder]
                @UseReferenceOrder,
                @ReleaseMediaToProductRelationships,
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated >= 0;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseTrackToProductRelationshipsOrderAsync(ReleaseTrackToProductRelationshipDto[] releaseTrackToProductRelationships, bool useReferenceOrder)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var i = 0;
        foreach (ReleaseTrackToProductRelationshipDto releaseTrackToProductRelationship in releaseTrackToProductRelationships)
        {
            if (!useReferenceOrder)
                releaseTrackToProductRelationship.Order = i++;
            else
                releaseTrackToProductRelationship.ReferenceOrder = i++;
        }

        using var releaseTrackToProductRelationshipsDataTable = new DataTable();
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.TrackNumber), typeof(byte));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.MediaNumber), typeof(byte));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.ReleaseId), typeof(Guid));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.ProductId), typeof(Guid));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.Name), typeof(string));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.Description), typeof(string));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.Order), typeof(int));
        releaseTrackToProductRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToProductRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseTrackToProductRelationshipDto releaseTrackToProductRelationship in releaseTrackToProductRelationships)
        {
            releaseTrackToProductRelationshipsDataTable.Rows.Add(
                releaseTrackToProductRelationship.TrackNumber.AsDbValue(),
                releaseTrackToProductRelationship.MediaNumber.AsDbValue(),
                releaseTrackToProductRelationship.ReleaseId.AsDbValue(),
                releaseTrackToProductRelationship.ProductId.AsDbValue(),
                releaseTrackToProductRelationship.Name.AsDbValue(),
                releaseTrackToProductRelationship.Description.AsDbValue(),
                releaseTrackToProductRelationship.Order.AsDbValue(),
                releaseTrackToProductRelationship.ReferenceOrder.AsDbValue());
        }

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter("UseReferenceOrder", useReferenceOrder.AsDbValue()),
            new SqlParameter("ReleaseTrackToProductRelationships", SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackToProductRelationship]", Value = releaseTrackToProductRelationshipsDataTable },
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateReleaseTrackToProductRelationshipsOrder]
                @UseReferenceOrder,
                @ReleaseTrackToProductRelationships,
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated >= 0;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateReleaseTrackToWorkRelationshipsOrderAsync(ReleaseTrackToWorkRelationshipDto[] releaseTrackToWorkRelationships, bool useReferenceOrder)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var i = 0;
        foreach (ReleaseTrackToWorkRelationshipDto releaseTrackToWorkRelationship in releaseTrackToWorkRelationships)
        {
            if (!useReferenceOrder)
                releaseTrackToWorkRelationship.Order = i++;
            else
                releaseTrackToWorkRelationship.ReferenceOrder = i++;
        }

        using var releaseTrackToWorkRelationshipsDataTable = new DataTable();
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.TrackNumber), typeof(byte));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.MediaNumber), typeof(byte));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.ReleaseId), typeof(Guid));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.WorkId), typeof(Guid));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.Name), typeof(string));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.Description), typeof(string));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.Order), typeof(int));
        releaseTrackToWorkRelationshipsDataTable.Columns.Add(nameof(ReleaseTrackToWorkRelationshipDto.ReferenceOrder), typeof(int));
        foreach (ReleaseTrackToWorkRelationshipDto releaseTrackToWorkRelationship in releaseTrackToWorkRelationships)
        {
            releaseTrackToWorkRelationshipsDataTable.Rows.Add(
                releaseTrackToWorkRelationship.TrackNumber.AsDbValue(),
                releaseTrackToWorkRelationship.MediaNumber.AsDbValue(),
                releaseTrackToWorkRelationship.ReleaseId.AsDbValue(),
                releaseTrackToWorkRelationship.WorkId.AsDbValue(),
                releaseTrackToWorkRelationship.Name.AsDbValue(),
                releaseTrackToWorkRelationship.Description.AsDbValue(),
                releaseTrackToWorkRelationship.Order.AsDbValue(),
                releaseTrackToWorkRelationship.ReferenceOrder.AsDbValue());
        }

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter("UseReferenceOrder", useReferenceOrder.AsDbValue()),
            new SqlParameter("ReleaseTrackToWorkRelationships", SqlDbType.Structured) { TypeName = "[dbo].[ReleaseTrackToWorkRelationship]", Value = releaseTrackToWorkRelationshipsDataTable },
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateReleaseTrackToWorkRelationshipsOrder]
                @UseReferenceOrder,
                @ReleaseTrackToWorkRelationships,
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated >= 0;
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

    private static void OrderReleaseToProductRelationships(ReleaseDto release)
    {
        release.ReleaseToProductRelationships = release.ReleaseToProductRelationships
            .OrderBy(releaseToProductRelationship => releaseToProductRelationship.Order)
            .ToList();
    }

    private static void OrderReleaseToReleaseGroupRelationships(ReleaseDto release)
    {
        release.ReleaseToReleaseGroupRelationships = release.ReleaseToReleaseGroupRelationships
            .OrderBy(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.Order)
            .ToList();
    }

    private static void OrderReleaseArtists(ReleaseDto release)
    {
        release.ReleaseArtists = release.ReleaseArtists
            .OrderBy(releaseArtist => releaseArtist.Order)
            .ToList();
    }

    private static void OrderReleaseFeaturedArtists(ReleaseDto release)
    {
        release.ReleaseFeaturedArtists = release.ReleaseFeaturedArtists
            .OrderBy(releaseFeaturedArtist => releaseFeaturedArtist.Order)
            .ToList();
    }

    private static void OrderReleasePerformers(ReleaseDto release)
    {
        release.ReleasePerformers = release.ReleasePerformers
            .OrderBy(releasePerformer => releasePerformer.Order)
            .ToList();
    }

    private static void OrderReleaseComposers(ReleaseDto release)
    {
        release.ReleaseComposers = release.ReleaseComposers
            .OrderBy(releaseComposer => releaseComposer.Order)
            .ToList();
    }

    private static void OrderReleaseGenres(ReleaseDto release)
    {
        release.ReleaseGenres = release.ReleaseGenres
            .OrderBy(releaseGenre => releaseGenre.Order)
            .ToList();
    }

    private static void OrderReleaseMediaCollection(ReleaseDto release)
    {
        release.ReleaseMediaCollection = release.ReleaseMediaCollection
            .OrderBy(releaseMedia => releaseMedia.MediaNumber)
            .ToList();
    }

    private static void OrderReleaseMediaToProductRelationships(ReleaseMediaDto releaseMedia)
    {
        releaseMedia.ReleaseMediaToProductRelationships = releaseMedia.ReleaseMediaToProductRelationships
            .OrderBy(releaseMediaToProductRelationship => releaseMediaToProductRelationship.Order)
            .ToList();
    }

    private static void OrderReleaseTrackCollection(ReleaseMediaDto releaseMedia)
    {
        releaseMedia.ReleaseTrackCollection = releaseMedia.ReleaseTrackCollection
            .OrderBy(releaseTrack => releaseTrack.TrackNumber)
            .ToList();
    }

    private static void OrderReleaseTrackToWorkRelationships(ReleaseTrackDto releaseTrack)
    {
        releaseTrack.ReleaseTrackToWorkRelationships = releaseTrack.ReleaseTrackToWorkRelationships
            .OrderBy(releaseTrackToWorkRelationship => releaseTrackToWorkRelationship.Order)
            .ToList();
    }

    private static void OrderReleaseTrackToProductRelationships(ReleaseTrackDto releaseTrack)
    {
        releaseTrack.ReleaseTrackToProductRelationships = releaseTrack.ReleaseTrackToProductRelationships
            .OrderBy(releaseTrackToProductRelationship => releaseTrackToProductRelationship.Order)
            .ToList();
    }

    private static void OrderReleaseTrackArtists(ReleaseTrackDto releaseTrack)
    {
        releaseTrack.ReleaseTrackArtists = releaseTrack.ReleaseTrackArtists
            .OrderBy(releaseTrackArtist => releaseTrackArtist.Order)
            .ToList();
    }

    private static void OrderReleaseTrackFeaturedArtists(ReleaseTrackDto releaseTrack)
    {
        releaseTrack.ReleaseTrackFeaturedArtists = releaseTrack.ReleaseTrackFeaturedArtists
            .OrderBy(releaseTrackFeaturedArtist => releaseTrackFeaturedArtist.Order)
            .ToList();
    }

    private static void OrderReleaseTrackPerformers(ReleaseTrackDto releaseTrack)
    {
        releaseTrack.ReleaseTrackPerformers = releaseTrack.ReleaseTrackPerformers
            .OrderBy(releaseTrackPerformer => releaseTrackPerformer.Order)
            .ToList();
    }

    private static void OrderReleaseTrackComposers(ReleaseTrackDto releaseTrack)
    {
        releaseTrack.ReleaseTrackComposers = releaseTrack.ReleaseTrackComposers
            .OrderBy(releaseTrackComposer => releaseTrackComposer.Order)
            .ToList();
    }

    private static void OrderReleaseTrackGenres(ReleaseTrackDto releaseTrack)
    {
        releaseTrack.ReleaseTrackGenres = releaseTrack.ReleaseTrackGenres
            .OrderBy(releaseTrackGenre => releaseTrackGenre.Order)
            .ToList();
    }

    private static void SetReleaseRelationshipOrders(IEnumerable<ReleaseRelationshipDto> releaseRelationships)
    {
        var i = 0;
        foreach (ReleaseRelationshipDto releaseRelationship in releaseRelationships)
        {
            releaseRelationship.Order = i++;
        }
    }

    private static void SetReleaseToProductRelationshipOrders(IEnumerable<ReleaseToProductRelationshipDto> releaseToProductRelationships)
    {
        var i = 0;
        foreach (ReleaseToProductRelationshipDto releaseToProductRelationship in releaseToProductRelationships)
        {
            releaseToProductRelationship.Order = i++;
        }
    }

    private static void SetReleaseToReleaseGroupRelationshipOrders(IEnumerable<ReleaseToReleaseGroupRelationshipDto> releaseToReleaseGroupRelationships)
    {
        var i = 0;
        foreach (ReleaseToReleaseGroupRelationshipDto releaseToReleaseGroupRelationship in releaseToReleaseGroupRelationships)
        {
            releaseToReleaseGroupRelationship.Order = i++;
        }
    }

    private static void SetReleaseArtistOrders(IEnumerable<ReleaseArtistDto> releaseArtists)
    {
        var i = 0;
        foreach (ReleaseArtistDto releaseArtist in releaseArtists)
        {
            releaseArtist.Order = i++;
        }
    }

    private static void SetReleaseFeaturedArtistOrders(IEnumerable<ReleaseFeaturedArtistDto> releaseFeaturedArtists)
    {
        var i = 0;
        foreach (ReleaseFeaturedArtistDto releaseFeaturedArtist in releaseFeaturedArtists)
        {
            releaseFeaturedArtist.Order = i++;
        }
    }

    private static void SetReleasePerformerOrders(IEnumerable<ReleasePerformerDto> releasePerformers)
    {
        var i = 0;
        foreach (ReleasePerformerDto releasePerformer in releasePerformers)
        {
            releasePerformer.Order = i++;
        }
    }

    private static void SetReleaseComposerOrders(IEnumerable<ReleaseComposerDto> releaseComposers)
    {
        var i = 0;
        foreach (ReleaseComposerDto releaseComposer in releaseComposers)
        {
            releaseComposer.Order = i++;
        }
    }

    private static void SetReleaseGenreOrders(IEnumerable<ReleaseGenreDto> releaseGenres)
    {
        var i = 0;
        foreach (ReleaseGenreDto releaseGenre in releaseGenres)
        {
            releaseGenre.Order = i++;
        }
    }

    private static void SetReleaseMediaForeignKeys(Guid releaseId, IEnumerable<ReleaseMediaDto> releaseMediaCollection)
    {
        foreach (ReleaseMediaDto releaseMedia in releaseMediaCollection)
        {
            releaseMedia.ReleaseId = releaseId;
        }
    }

    private static void SetReleaseMediaToProductRelationshipOrders(byte mediaNumber, Guid releaseId, IEnumerable<ReleaseMediaToProductRelationshipDto> releaseMediaToProductRelationships)
    {
        var i = 0;
        foreach (ReleaseMediaToProductRelationshipDto releaseMediaToProductRelationship in releaseMediaToProductRelationships)
        {
            releaseMediaToProductRelationship.MediaNumber = mediaNumber;
            releaseMediaToProductRelationship.ReleaseId = releaseId;
            releaseMediaToProductRelationship.Order = i++;
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

    private static void SetReleaseTrackToProductRelationshipOrders(byte trackNumber, byte mediaNumber, Guid releaseId, IEnumerable<ReleaseTrackToProductRelationshipDto> releaseTrackToProductRelationships)
    {
        var i = 0;
        foreach (ReleaseTrackToProductRelationshipDto releaseTrackToProductRelationship in releaseTrackToProductRelationships)
        {
            releaseTrackToProductRelationship.TrackNumber = trackNumber;
            releaseTrackToProductRelationship.MediaNumber = mediaNumber;
            releaseTrackToProductRelationship.ReleaseId = releaseId;
            releaseTrackToProductRelationship.Order = i++;
        }
    }

    private static void SetReleaseTrackToWorkRelationshipOrders(byte trackNumber, byte mediaNumber, Guid releaseId, IEnumerable<ReleaseTrackToWorkRelationshipDto> releaseTrackToWorkRelationships)
    {
        var i = 0;
        foreach (ReleaseTrackToWorkRelationshipDto releaseTrackToWorkRelationship in releaseTrackToWorkRelationships)
        {
            releaseTrackToWorkRelationship.TrackNumber = trackNumber;
            releaseTrackToWorkRelationship.MediaNumber = mediaNumber;
            releaseTrackToWorkRelationship.ReleaseId = releaseId;
            releaseTrackToWorkRelationship.Order = i++;
        }
    }

    private static void SetReleaseTrackArtistOrders(byte trackNumber, byte mediaNumber, Guid releaseId, IEnumerable<ReleaseTrackArtistDto> releaseTrackArtists)
    {
        var i = 0;
        foreach (ReleaseTrackArtistDto releaseTrackArtist in releaseTrackArtists)
        {
            releaseTrackArtist.TrackNumber = trackNumber;
            releaseTrackArtist.MediaNumber = mediaNumber;
            releaseTrackArtist.ReleaseId = releaseId;
            releaseTrackArtist.Order = i++;
        }
    }

    private static void SetReleaseTrackFeaturedArtistOrders(byte trackNumber, byte mediaNumber, Guid releaseId, IEnumerable<ReleaseTrackFeaturedArtistDto> releaseTrackFeaturedArtists)
    {
        var i = 0;
        foreach (ReleaseTrackFeaturedArtistDto releaseTrackFeaturedArtist in releaseTrackFeaturedArtists)
        {
            releaseTrackFeaturedArtist.TrackNumber = trackNumber;
            releaseTrackFeaturedArtist.MediaNumber = mediaNumber;
            releaseTrackFeaturedArtist.ReleaseId = releaseId;
            releaseTrackFeaturedArtist.Order = i++;
        }
    }

    private static void SetReleaseTrackPerformerOrders(byte trackNumber, byte mediaNumber, Guid releaseId, IEnumerable<ReleaseTrackPerformerDto> releaseTrackPerformers)
    {
        var i = 0;
        foreach (ReleaseTrackPerformerDto releaseTrackPerformer in releaseTrackPerformers)
        {
            releaseTrackPerformer.TrackNumber = trackNumber;
            releaseTrackPerformer.MediaNumber = mediaNumber;
            releaseTrackPerformer.ReleaseId = releaseId;
            releaseTrackPerformer.Order = i++;
        }
    }

    private static void SetReleaseTrackComposerOrders(byte trackNumber, byte mediaNumber, Guid releaseId, IEnumerable<ReleaseTrackComposerDto> releaseTrackComposers)
    {
        var i = 0;
        foreach (ReleaseTrackComposerDto releaseTrackComposer in releaseTrackComposers)
        {
            releaseTrackComposer.TrackNumber = trackNumber;
            releaseTrackComposer.MediaNumber = mediaNumber;
            releaseTrackComposer.ReleaseId = releaseId;
            releaseTrackComposer.Order = i++;
        }
    }

    private static void SetReleaseTrackGenreOrders(byte trackNumber, byte mediaNumber, Guid releaseId, IEnumerable<ReleaseTrackGenreDto> releaseTrackGenres)
    {
        var i = 0;
        foreach (ReleaseTrackGenreDto releaseTrackGenre in releaseTrackGenres)
        {
            releaseTrackGenre.TrackNumber = trackNumber;
            releaseTrackGenre.MediaNumber = mediaNumber;
            releaseTrackGenre.ReleaseId = releaseId;
            releaseTrackGenre.Order = i++;
        }
    }
}
