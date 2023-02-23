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
/// Represents a SQL-Server-specific implementation of the artist repository.
/// </summary>
public class SqlServerArtistRepository : IArtistRepository
{
    private readonly IDbContextFactory<CatalogServiceDbContext> m_contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerArtistRepository" /> type using the specified services.
    /// </summary>
    /// <param name="contextFactory">The database context factory.</param>
    public SqlServerArtistRepository(IDbContextFactory<CatalogServiceDbContext> contextFactory)
    {
        m_contextFactory = contextFactory;
    }

    /// <inheritdoc />
    public async Task<ArtistDto?> GetArtistAsync(Guid artistId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var artistIdParameter = new SqlParameter("ArtistId", artistId.AsDbValue());

        var query = $"SELECT * FROM [dbo].[ufn_GetArtist] (@{artistIdParameter.ParameterName})";

        ArtistDto? artist = await context.Artists.FromSqlRaw(query, artistIdParameter).AsNoTracking()
            .Include(artist => artist.ArtistRelationships)
            .ThenInclude(artistRelationship => artistRelationship.DependentArtist)
            .Include(artist => artist.ArtistGenres)
            .ThenInclude(artistGenre => artistGenre.Genre)
            .FirstOrDefaultAsync();

        if (artist is not null)
        {
            OrderArtistRelationships(artist);
            OrderArtistGenres(artist);
        }

        return artist;
    }

    /// <inheritdoc />
    public async Task<ArtistDto[]> GetArtistsAsync()
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ArtistDto[] artists = await context.Artists.AsNoTracking()
            .Include(artist => artist.ArtistRelationships)
            .ThenInclude(artistRelationship => artistRelationship.DependentArtist)
            .Include(artist => artist.ArtistGenres)
            .ThenInclude(artistGenre => artistGenre.Genre)
            .ToArrayAsync();

        foreach (ArtistDto artist in artists)
        {
            OrderArtistRelationships(artist);
            OrderArtistGenres(artist);
        }

        return artists;
    }

    /// <inheritdoc />
    public async Task<ArtistDto[]> GetArtistsAsync(IEnumerable<Guid> artistIds)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        using var artistIdsDataTable = new DataTable();
        artistIdsDataTable.Columns.Add("Value", typeof(Guid));
        foreach (Guid artistId in artistIds)
            artistIdsDataTable.Rows.Add(artistId.AsDbValue());

        var artistIdsParameter = new SqlParameter("ArtistIds", SqlDbType.Structured) { TypeName = "[dbo].[GuidArray]", Value = artistIdsDataTable };

        var query = $"SELECT * FROM [dbo].[ufn_GetArtists] (@{artistIdsParameter.ParameterName})";

        ArtistDto[] artists = await context.Artists.FromSqlRaw(query, artistIdsParameter).AsNoTracking()
            .Include(artist => artist.ArtistRelationships)
            .ThenInclude(artistRelationship => artistRelationship.DependentArtist)
            .Include(artist => artist.ArtistGenres)
            .ThenInclude(artistGenre => artistGenre.Genre)
            .ToArrayAsync();

        foreach (ArtistDto artist in artists)
        {
            OrderArtistRelationships(artist);
            OrderArtistGenres(artist);
        }

        return artists;
    }

    /// <inheritdoc />
    public async Task<ArtistDto[]> GetArtistsAsync(EntityCollectionProcessor<ArtistDto> collectionProcessor)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ArtistDto[] artists = await collectionProcessor(context.Artists.AsNoTracking())
            .Include(artist => artist.ArtistRelationships)
            .ThenInclude(artistRelationship => artistRelationship.DependentArtist)
            .Include(artist => artist.ArtistGenres)
            .ThenInclude(artistGenre => artistGenre.Genre)
            .ToArrayAsync();

        foreach (ArtistDto artist in artists)
        {
            OrderArtistRelationships(artist);
            OrderArtistGenres(artist);
        }

        return artists;
    }

    /// <inheritdoc />
    public async Task<PageResponseDto<ArtistDto>> GetArtistsAsync(ArtistPageRequestDto artistPageRequest)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<ArtistDto> baseCollection = context.Artists.AsNoTracking();

        if (artistPageRequest.Name is not null)
            baseCollection = baseCollection.Where(artist => artist.Name.Contains(artistPageRequest.Name));

        if (artistPageRequest.Enabled is not null)
            baseCollection = baseCollection.Where(artist => artist.Enabled == (bool)artistPageRequest.Enabled);

        var totalCount = await baseCollection.CountAsync();
        List<ArtistDto> artists = await baseCollection
            .Include(artist => artist.ArtistRelationships)
            .ThenInclude(artistRelationship => artistRelationship.DependentArtist)
            .Include(artist => artist.ArtistGenres)
            .ThenInclude(artistGenre => artistGenre.Genre)
            .OrderByDescending(artist => artist.SystemEntity)
            .ThenBy(artist => artist.Name)
            .Skip(artistPageRequest.PageSize * artistPageRequest.PageIndex)
            .Take(artistPageRequest.PageSize)
            .ToListAsync();

        foreach (ArtistDto artist in artists)
        {
            OrderArtistRelationships(artist);
            OrderArtistGenres(artist);
        }

        return new PageResponseDto<ArtistDto>()
        {
            PageSize = artistPageRequest.PageSize,
            PageIndex = artistPageRequest.PageIndex,
            TotalCount = totalCount,
            Items = artists,
        };
    }

    /// <inheritdoc />
    public async Task<ArtistRelationshipDto[]> GetArtistRelationshipsAsync(Guid artistId, bool includeReverseRelationships = false)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<ArtistRelationshipDto> baseCollection = context.ArtistRelationships.AsNoTracking()
            .Include(artistRelationship => artistRelationship.Artist)
            .Include(artistRelationship => artistRelationship.DependentArtist);

        ArtistRelationshipDto[] artistRelationships = await baseCollection
            .Where(artistRelationship => artistRelationship.ArtistId == artistId)
            .OrderBy(artistRelationship => artistRelationship.Order)
            .ToArrayAsync();

        if (includeReverseRelationships)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            ArtistRelationshipDto[] reverseRelationships = await baseCollection
                .Where(artistRelationship => artistRelationship.DependentArtistId == artistId)
                .OrderBy(artistRelationship => artistRelationship.Artist.Name)
                .ToArrayAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            artistRelationships = artistRelationships.Concat(reverseRelationships).ToArray();
        }

        return artistRelationships;
    }

    /// <inheritdoc />
    public async Task<ArtistDto> CreateArtistAsync(ArtistDto artist)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SetArtistRelationshipOrders(artist.ArtistRelationships);
        SetArtistGenreOrders(artist.ArtistGenres);

        using var artistRelationshipsDataTable = new DataTable();
        artistRelationshipsDataTable.Columns.Add(nameof(ArtistRelationshipDto.ArtistId), typeof(Guid));
        artistRelationshipsDataTable.Columns.Add(nameof(ArtistRelationshipDto.DependentArtistId), typeof(Guid));
        artistRelationshipsDataTable.Columns.Add(nameof(ArtistRelationshipDto.Name), typeof(string));
        artistRelationshipsDataTable.Columns.Add(nameof(ArtistRelationshipDto.Description), typeof(string));
        artistRelationshipsDataTable.Columns.Add(nameof(ArtistRelationshipDto.Order), typeof(int));
        foreach (ArtistRelationshipDto artistRelationship in artist.ArtistRelationships)
        {
            artistRelationshipsDataTable.Rows.Add(
                artistRelationship.ArtistId.AsDbValue(),
                artistRelationship.DependentArtistId.AsDbValue(),
                artistRelationship.Name.AsDbValue(),
                artistRelationship.Description.AsDbValue(),
                artistRelationship.Order.AsDbValue());
        }

        using var artistGenresDataTable = new DataTable();
        artistGenresDataTable.Columns.Add(nameof(ArtistGenreDto.ArtistId), typeof(Guid));
        artistGenresDataTable.Columns.Add(nameof(ArtistGenreDto.GenreId), typeof(Guid));
        artistGenresDataTable.Columns.Add(nameof(ArtistGenreDto.Order), typeof(int));
        foreach (ArtistGenreDto artistGenre in artist.ArtistGenres)
        {
            artistGenresDataTable.Rows.Add(
                artistGenre.ArtistId.AsDbValue(),
                artistGenre.GenreId.AsDbValue(),
                artistGenre.Order.AsDbValue());
        }

        SqlParameter resultIdParameter;
        SqlParameter resultCreatedOnParameter;
        SqlParameter resultUpdatedOnParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ArtistDto.Id), artist.Id.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.Name), artist.Name.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.Description), artist.Description.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.DisambiguationText), artist.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.SystemEntity), artist.SystemEntity.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.Enabled), artist.Enabled.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.ArtistRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ArtistRelationship]", Value = artistRelationshipsDataTable },
            new SqlParameter(nameof(ArtistDto.ArtistGenres), SqlDbType.Structured) { TypeName = "[dbo].[ArtistGenre]", Value = artistGenresDataTable },
            resultIdParameter = new SqlParameter($"Result{nameof(ArtistDto.Id)}", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output },
            resultCreatedOnParameter = new SqlParameter($"Result{nameof(ArtistDto.CreatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
            resultUpdatedOnParameter = new SqlParameter($"Result{nameof(ArtistDto.UpdatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_CreateArtist]
                @{nameof(ArtistDto.Id)},
                @{nameof(ArtistDto.Name)},
                @{nameof(ArtistDto.Description)},
                @{nameof(ArtistDto.DisambiguationText)},
                @{nameof(ArtistDto.SystemEntity)},
                @{nameof(ArtistDto.Enabled)},
                @{nameof(ArtistDto.ArtistRelationships)},
                @{nameof(ArtistDto.ArtistGenres)},
                @{resultIdParameter.ParameterName} OUTPUT,
                @{resultCreatedOnParameter.ParameterName} OUTPUT,
                @{resultUpdatedOnParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        artist.Id = (Guid)resultIdParameter.Value;
        artist.CreatedOn = (DateTimeOffset)resultCreatedOnParameter.Value;
        artist.UpdatedOn = (DateTimeOffset)resultUpdatedOnParameter.Value;

        return artist;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateArtistAsync(ArtistDto artist)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SetArtistRelationshipOrders(artist.ArtistRelationships);
        SetArtistGenreOrders(artist.ArtistGenres);

        using var artistRelationshipsDataTable = new DataTable();
        artistRelationshipsDataTable.Columns.Add(nameof(ArtistRelationshipDto.ArtistId), typeof(Guid));
        artistRelationshipsDataTable.Columns.Add(nameof(ArtistRelationshipDto.DependentArtistId), typeof(Guid));
        artistRelationshipsDataTable.Columns.Add(nameof(ArtistRelationshipDto.Name), typeof(string));
        artistRelationshipsDataTable.Columns.Add(nameof(ArtistRelationshipDto.Description), typeof(string));
        artistRelationshipsDataTable.Columns.Add(nameof(ArtistRelationshipDto.Order), typeof(int));
        foreach (ArtistRelationshipDto artistRelationship in artist.ArtistRelationships)
        {
            artistRelationshipsDataTable.Rows.Add(
                artistRelationship.ArtistId.AsDbValue(),
                artistRelationship.DependentArtistId.AsDbValue(),
                artistRelationship.Name.AsDbValue(),
                artistRelationship.Description.AsDbValue(),
                artistRelationship.Order.AsDbValue());
        }

        using var artistGenresDataTable = new DataTable();
        artistGenresDataTable.Columns.Add(nameof(ArtistGenreDto.ArtistId), typeof(Guid));
        artistGenresDataTable.Columns.Add(nameof(ArtistGenreDto.GenreId), typeof(Guid));
        artistGenresDataTable.Columns.Add(nameof(ArtistGenreDto.Order), typeof(int));
        foreach (ArtistGenreDto artistGenre in artist.ArtistGenres)
        {
            artistGenresDataTable.Rows.Add(
                artistGenre.ArtistId.AsDbValue(),
                artistGenre.GenreId.AsDbValue(),
                artistGenre.Order.AsDbValue());
        }

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ArtistDto.Id), artist.Id.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.Name), artist.Name.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.Description), artist.Description.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.DisambiguationText), artist.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.SystemEntity), artist.SystemEntity.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.Enabled), artist.Enabled.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.ArtistRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ArtistRelationship]", Value = artistRelationshipsDataTable },
            new SqlParameter(nameof(ArtistDto.ArtistGenres), SqlDbType.Structured) { TypeName = "[dbo].[ArtistGenre]", Value = artistGenresDataTable },
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateArtist]
                @{nameof(ArtistDto.Id)},
                @{nameof(ArtistDto.Name)},
                @{nameof(ArtistDto.Description)},
                @{nameof(ArtistDto.DisambiguationText)},
                @{nameof(ArtistDto.SystemEntity)},
                @{nameof(ArtistDto.Enabled)},
                @{nameof(ArtistDto.ArtistRelationships)},
                @{nameof(ArtistDto.ArtistGenres)},
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteArtistAsync(Guid artistId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultRowsDeletedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ArtistDto.Id), artistId.AsDbValue()),
            resultRowsDeletedParameter = new SqlParameter("ResultRowsDeleted", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_DeleteArtist]
                @{nameof(ArtistDto.Id)},
                @{resultRowsDeletedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsDeleted = (int)resultRowsDeletedParameter.Value;
        return rowsDeleted > 0;
    }

    private static void OrderArtistRelationships(ArtistDto artist)
    {
        artist.ArtistRelationships = artist.ArtistRelationships
            .OrderBy(artistRelationship => artistRelationship.Order)
            .ToList();
    }

    private static void OrderArtistGenres(ArtistDto artist)
    {
        artist.ArtistGenres = artist.ArtistGenres
            .OrderBy(artistGenre => artistGenre.Order)
            .ToList();
    }

    private static void SetArtistRelationshipOrders(IEnumerable<ArtistRelationshipDto> artistRelationships)
    {
        var i = 0;
        foreach (ArtistRelationshipDto artistRelationship in artistRelationships)
        {
            artistRelationship.Order = i++;
        }
    }

    private static void SetArtistGenreOrders(IEnumerable<ArtistGenreDto> artistGenres)
    {
        var i = 0;
        foreach (ArtistGenreDto artistGenre in artistGenres)
        {
            artistGenre.Order = i++;
        }
    }
}
