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
/// Represents a SQL Server - specific implementation of the artist repository.
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
            .FirstOrDefaultAsync();

        return artist;
    }

    /// <inheritdoc />
    public async Task<ArtistDto[]> GetArtistsAsync()
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ArtistDto[] artists = await context.Artists.AsNoTracking()
            .ToArrayAsync();

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
            .ToArrayAsync();

        return artists;
    }

    /// <inheritdoc />
    public async Task<ArtistDto[]> GetArtistsAsync(EntityCollectionProcessor<ArtistDto> collectionProcessor)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ArtistDto[] artists = await collectionProcessor(context.Artists.AsNoTracking())
            .ToArrayAsync();

        return artists;
    }

    /// <inheritdoc />
    public async Task<PageResponseDto<ArtistDto>> GetArtistsAsync(ArtistRequestDto artistRequest)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<ArtistDto> baseCollection = context.Artists.AsNoTracking();

        if (artistRequest.Name is not null)
            baseCollection = baseCollection.Where(artist => artist.Name.Contains(artistRequest.Name));

        if (artistRequest.Enabled is not null)
            baseCollection = baseCollection.Where(artist => artist.Enabled == (bool)artistRequest.Enabled);

        var totalCount = await baseCollection.CountAsync();
        List<ArtistDto> artists = await baseCollection
            .OrderByDescending(artist => artist.SystemEntity)
            .ThenBy(artist => artist.Name)
            .Skip(artistRequest.PageSize * artistRequest.PageIndex)
            .Take(artistRequest.PageSize)
            .ToListAsync();

        return new PageResponseDto<ArtistDto>()
        {
            PageSize = artistRequest.PageSize,
            PageIndex = artistRequest.PageIndex,
            TotalCount = totalCount,
            Items = artists,
        };
    }

    /// <inheritdoc />
    public async Task<ArtistDto> CreateArtistAsync(ArtistDto artist)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

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

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ArtistDto.Id), artist.Id.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.Name), artist.Name.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.Description), artist.Description.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.DisambiguationText), artist.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.SystemEntity), artist.SystemEntity.AsDbValue()),
            new SqlParameter(nameof(ArtistDto.Enabled), artist.Enabled.AsDbValue()),
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
}
