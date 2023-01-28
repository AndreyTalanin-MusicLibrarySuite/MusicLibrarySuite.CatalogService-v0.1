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
            .Include(release => release.ReleaseToProductRelationships)
            .ThenInclude(releaseToProductRelationship => releaseToProductRelationship.Product)
            .Include(release => release.ReleaseToReleaseGroupRelationships)
            .ThenInclude(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.ReleaseGroup)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .FirstOrDefaultAsync();

        if (release is not null)
        {
            OrderReleaseRelationships(release);
            OrderReleaseArtists(release);
            OrderReleaseFeaturedArtists(release);
            OrderReleasePerformers(release);
            OrderReleaseComposers(release);
            OrderReleaseGenres(release);
            OrderReleaseToProductRelationships(release);
            OrderReleaseToReleaseGroupRelationships(release);
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
            .Include(release => release.ReleaseToProductRelationships)
            .ThenInclude(releaseToProductRelationship => releaseToProductRelationship.Product)
            .Include(release => release.ReleaseToReleaseGroupRelationships)
            .ThenInclude(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.ReleaseGroup)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ToArrayAsync();

        foreach (ReleaseDto release in releases)
        {
            OrderReleaseRelationships(release);
            OrderReleaseArtists(release);
            OrderReleaseFeaturedArtists(release);
            OrderReleasePerformers(release);
            OrderReleaseComposers(release);
            OrderReleaseGenres(release);
            OrderReleaseToProductRelationships(release);
            OrderReleaseToReleaseGroupRelationships(release);
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
            .Include(release => release.ReleaseToProductRelationships)
            .ThenInclude(releaseToProductRelationship => releaseToProductRelationship.Product)
            .Include(release => release.ReleaseToReleaseGroupRelationships)
            .ThenInclude(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.ReleaseGroup)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ToArrayAsync();

        foreach (ReleaseDto release in releases)
        {
            OrderReleaseRelationships(release);
            OrderReleaseArtists(release);
            OrderReleaseFeaturedArtists(release);
            OrderReleasePerformers(release);
            OrderReleaseComposers(release);
            OrderReleaseGenres(release);
            OrderReleaseToProductRelationships(release);
            OrderReleaseToReleaseGroupRelationships(release);
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
            .Include(release => release.ReleaseToProductRelationships)
            .ThenInclude(releaseToProductRelationship => releaseToProductRelationship.Product)
            .Include(release => release.ReleaseToReleaseGroupRelationships)
            .ThenInclude(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.ReleaseGroup)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .ToArrayAsync();

        foreach (ReleaseDto release in releases)
        {
            OrderReleaseRelationships(release);
            OrderReleaseArtists(release);
            OrderReleaseFeaturedArtists(release);
            OrderReleasePerformers(release);
            OrderReleaseComposers(release);
            OrderReleaseGenres(release);
            OrderReleaseToProductRelationships(release);
            OrderReleaseToReleaseGroupRelationships(release);
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
            .Include(release => release.ReleaseToProductRelationships)
            .ThenInclude(releaseToProductRelationship => releaseToProductRelationship.Product)
            .Include(release => release.ReleaseToReleaseGroupRelationships)
            .ThenInclude(releaseToReleaseGroupRelationship => releaseToReleaseGroupRelationship.ReleaseGroup)
            .Include(release => release.ReleaseMediaCollection)
            .ThenInclude(releaseMedia => releaseMedia.ReleaseTrackCollection)
            .OrderBy(release => release.Title)
            .Skip(releaseRequest.PageSize * releaseRequest.PageIndex)
            .Take(releaseRequest.PageSize)
            .ToListAsync();

        foreach (ReleaseDto release in releases)
        {
            OrderReleaseRelationships(release);
            OrderReleaseArtists(release);
            OrderReleaseFeaturedArtists(release);
            OrderReleasePerformers(release);
            OrderReleaseComposers(release);
            OrderReleaseGenres(release);
            OrderReleaseToProductRelationships(release);
            OrderReleaseToReleaseGroupRelationships(release);
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<ReleaseDto> CreateReleaseAsync(ReleaseDto release)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SetReleaseRelationshipOrders(release.ReleaseRelationships);
        SetReleaseArtistOrders(release.ReleaseArtists);
        SetReleaseFeaturedArtistOrders(release.ReleaseFeaturedArtists);
        SetReleasePerformerOrders(release.ReleasePerformers);
        SetReleaseComposerOrders(release.ReleaseComposers);
        SetReleaseGenreOrders(release.ReleaseGenres);
        SetReleaseToProductRelationshipOrders(release.ReleaseToProductRelationships);
        SetReleaseToReleaseGroupRelationshipOrders(release.ReleaseToReleaseGroupRelationships);
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
            new SqlParameter(nameof(ReleaseDto.ReleaseArtists), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseArtist]", Value = releaseArtistsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseFeaturedArtists), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseFeaturedArtist]", Value = releaseFeaturedArtistsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleasePerformers), SqlDbType.Structured) { TypeName = "[dbo].[ReleasePerformer]", Value = releasePerformersDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseComposers), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseComposer]", Value = releaseComposersDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseGenres), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseGenre]", Value = releaseGenresDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseToProductRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseToProductRelationship]", Value = releaseToProductRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseToReleaseGroupRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseToReleaseGroupRelationship]", Value = releaseToReleaseGroupRelationshipsDataTable },
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
                @{nameof(ReleaseDto.ReleaseArtists)},
                @{nameof(ReleaseDto.ReleaseFeaturedArtists)},
                @{nameof(ReleaseDto.ReleasePerformers)},
                @{nameof(ReleaseDto.ReleaseComposers)},
                @{nameof(ReleaseDto.ReleaseGenres)},
                @{nameof(ReleaseDto.ReleaseToProductRelationships)},
                @{nameof(ReleaseDto.ReleaseToReleaseGroupRelationships)},
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
        SetReleaseArtistOrders(release.ReleaseArtists);
        SetReleaseFeaturedArtistOrders(release.ReleaseFeaturedArtists);
        SetReleasePerformerOrders(release.ReleasePerformers);
        SetReleaseComposerOrders(release.ReleaseComposers);
        SetReleaseGenreOrders(release.ReleaseGenres);
        SetReleaseToProductRelationshipOrders(release.ReleaseToProductRelationships);
        SetReleaseToReleaseGroupRelationshipOrders(release.ReleaseToReleaseGroupRelationships);
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
            new SqlParameter(nameof(ReleaseDto.ReleaseArtists), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseArtist]", Value = releaseArtistsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseFeaturedArtists), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseFeaturedArtist]", Value = releaseFeaturedArtistsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleasePerformers), SqlDbType.Structured) { TypeName = "[dbo].[ReleasePerformer]", Value = releasePerformersDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseComposers), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseComposer]", Value = releaseComposersDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseGenres), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseGenre]", Value = releaseGenresDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseToProductRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseToProductRelationship]", Value = releaseToProductRelationshipsDataTable },
            new SqlParameter(nameof(ReleaseDto.ReleaseToReleaseGroupRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ReleaseToReleaseGroupRelationship]", Value = releaseToReleaseGroupRelationshipsDataTable },
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
                @{nameof(ReleaseDto.ReleaseArtists)},
                @{nameof(ReleaseDto.ReleaseFeaturedArtists)},
                @{nameof(ReleaseDto.ReleasePerformers)},
                @{nameof(ReleaseDto.ReleaseComposers)},
                @{nameof(ReleaseDto.ReleaseGenres)},
                @{nameof(ReleaseDto.ReleaseToProductRelationships)},
                @{nameof(ReleaseDto.ReleaseToReleaseGroupRelationships)},
                @{nameof(ReleaseDto.ReleaseMediaCollection)},
                @{nameof(ReleaseMediaDto.ReleaseTrackCollection)},
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

    private static void SetReleaseArtistOrders(ICollection<ReleaseArtistDto> releaseArtists)
    {
        var i = 0;
        foreach (ReleaseArtistDto releaseArtist in releaseArtists)
        {
            releaseArtist.Order = i++;
        }
    }

    private static void SetReleaseFeaturedArtistOrders(ICollection<ReleaseFeaturedArtistDto> releaseFeaturedArtists)
    {
        var i = 0;
        foreach (ReleaseFeaturedArtistDto releaseFeaturedArtist in releaseFeaturedArtists)
        {
            releaseFeaturedArtist.Order = i++;
        }
    }

    private static void SetReleasePerformerOrders(ICollection<ReleasePerformerDto> releasePerformers)
    {
        var i = 0;
        foreach (ReleasePerformerDto releasePerformer in releasePerformers)
        {
            releasePerformer.Order = i++;
        }
    }

    private static void SetReleaseComposerOrders(ICollection<ReleaseComposerDto> releaseComposers)
    {
        var i = 0;
        foreach (ReleaseComposerDto releaseComposer in releaseComposers)
        {
            releaseComposer.Order = i++;
        }
    }

    private static void SetReleaseGenreOrders(ICollection<ReleaseGenreDto> releaseGenres)
    {
        var i = 0;
        foreach (ReleaseGenreDto releaseGenre in releaseGenres)
        {
            releaseGenre.Order = i++;
        }
    }

    private static void SetReleaseToProductRelationshipOrders(ICollection<ReleaseToProductRelationshipDto> releaseToProductRelationships)
    {
        var i = 0;
        foreach (ReleaseToProductRelationshipDto releaseToProductRelationship in releaseToProductRelationships)
        {
            releaseToProductRelationship.Order = i++;
        }
    }

    private static void SetReleaseToReleaseGroupRelationshipOrders(ICollection<ReleaseToReleaseGroupRelationshipDto> releaseToReleaseGroupRelationships)
    {
        var i = 0;
        foreach (ReleaseToReleaseGroupRelationshipDto releaseToReleaseGroupRelationship in releaseToReleaseGroupRelationships)
        {
            releaseToReleaseGroupRelationship.Order = i++;
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
