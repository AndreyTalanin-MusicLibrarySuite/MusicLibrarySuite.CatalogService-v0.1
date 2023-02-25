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
/// Represents a SQL-Server-specific implementation of the genre repository.
/// </summary>
public class SqlServerGenreRepository : IGenreRepository
{
    private readonly IDbContextFactory<CatalogServiceDbContext> m_contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerGenreRepository" /> type using the specified services.
    /// </summary>
    /// <param name="contextFactory">The database context factory.</param>
    public SqlServerGenreRepository(IDbContextFactory<CatalogServiceDbContext> contextFactory)
    {
        m_contextFactory = contextFactory;
    }

    /// <inheritdoc />
    public async Task<GenreDto?> GetGenreAsync(Guid genreId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var genreIdParameter = new SqlParameter("GenreId", genreId.AsDbValue());

        var query = $"SELECT * FROM [dbo].[ufn_GetGenre] (@{genreIdParameter.ParameterName})";

        GenreDto? genre = await context.Genres
            .FromSqlRaw(query, genreIdParameter)
            .Include(genre => genre.GenreRelationships)
            .ThenInclude(genreRelationship => genreRelationship.DependentGenre)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync();

        if (genre is not null)
        {
            OrderGenreRelationships(genre);
        }

        return genre;
    }

    /// <inheritdoc />
    public async Task<GenreDto[]> GetGenresAsync()
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        GenreDto[] genres = await context.Genres
            .AsNoTracking()
            .ToArrayAsync();

        return genres;
    }

    /// <inheritdoc />
    public async Task<GenreDto[]> GetGenresAsync(IEnumerable<Guid> genreIds)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        using var genreIdsDataTable = new DataTable();
        genreIdsDataTable.Columns.Add("Value", typeof(Guid));
        foreach (Guid genreId in genreIds)
            genreIdsDataTable.Rows.Add(genreId.AsDbValue());

        var genreIdsParameter = new SqlParameter("GenreIds", SqlDbType.Structured) { TypeName = "[dbo].[GuidArray]", Value = genreIdsDataTable };

        var query = $"SELECT * FROM [dbo].[ufn_GetGenres] (@{genreIdsParameter.ParameterName})";

        GenreDto[] genres = await context.Genres
            .FromSqlRaw(query, genreIdsParameter)
            .AsNoTracking()
            .ToArrayAsync();

        return genres;
    }

    /// <inheritdoc />
    public async Task<GenreDto[]> GetGenresAsync(EntityCollectionProcessor<GenreDto> collectionProcessor)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        GenreDto[] genres = await collectionProcessor(context.Genres)
            .AsNoTracking()
            .ToArrayAsync();

        return genres;
    }

    /// <inheritdoc />
    public async Task<PageResponseDto<GenreDto>> GetGenresAsync(GenrePageRequestDto genrePageRequest)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<GenreDto> baseCollection = context.Genres;

        if (genrePageRequest.Name is not null)
            baseCollection = baseCollection.Where(genre => genre.Name.Contains(genrePageRequest.Name));

        if (genrePageRequest.Enabled is not null)
            baseCollection = baseCollection.Where(genre => genre.Enabled == (bool)genrePageRequest.Enabled);

        var totalCount = await baseCollection.CountAsync();
        List<GenreDto> genres = await baseCollection
            .OrderByDescending(genre => genre.SystemEntity)
            .ThenBy(genre => genre.Name)
            .Skip(genrePageRequest.PageSize * genrePageRequest.PageIndex)
            .Take(genrePageRequest.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PageResponseDto<GenreDto>()
        {
            PageSize = genrePageRequest.PageSize,
            PageIndex = genrePageRequest.PageIndex,
            TotalCount = totalCount,
            Items = genres,
        };
    }

    /// <inheritdoc />
    public async Task<GenreRelationshipDto[]> GetGenreRelationshipsAsync(Guid genreId, bool includeReverseRelationships = false)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<GenreRelationshipDto> baseCollection = context.GenreRelationships.AsNoTracking()
            .Include(genreRelationship => genreRelationship.Genre)
            .Include(genreRelationship => genreRelationship.DependentGenre);

        GenreRelationshipDto[] genreRelationships = await baseCollection
            .Where(genreRelationship => genreRelationship.GenreId == genreId)
            .OrderBy(genreRelationship => genreRelationship.Order)
            .ToArrayAsync();

        if (includeReverseRelationships)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            GenreRelationshipDto[] reverseRelationships = await baseCollection
                .Where(genreRelationship => genreRelationship.DependentGenreId == genreId)
                .OrderBy(genreRelationship => genreRelationship.Genre.Name)
                .ToArrayAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            genreRelationships = genreRelationships.Concat(reverseRelationships).ToArray();
        }

        return genreRelationships;
    }

    /// <inheritdoc />
    public async Task<GenreDto> CreateGenreAsync(GenreDto genre)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SetGenreRelationshipOrders(genre.GenreRelationships);

        using var genreRelationshipsDataTable = new DataTable();
        genreRelationshipsDataTable.Columns.Add(nameof(GenreRelationshipDto.GenreId), typeof(Guid));
        genreRelationshipsDataTable.Columns.Add(nameof(GenreRelationshipDto.DependentGenreId), typeof(Guid));
        genreRelationshipsDataTable.Columns.Add(nameof(GenreRelationshipDto.Name), typeof(string));
        genreRelationshipsDataTable.Columns.Add(nameof(GenreRelationshipDto.Description), typeof(string));
        genreRelationshipsDataTable.Columns.Add(nameof(GenreRelationshipDto.Order), typeof(int));
        foreach (GenreRelationshipDto genreRelationship in genre.GenreRelationships)
        {
            genreRelationshipsDataTable.Rows.Add(
                genreRelationship.GenreId.AsDbValue(),
                genreRelationship.DependentGenreId.AsDbValue(),
                genreRelationship.Name.AsDbValue(),
                genreRelationship.Description.AsDbValue(),
                genreRelationship.Order.AsDbValue());
        }

        SqlParameter resultIdParameter;
        SqlParameter resultCreatedOnParameter;
        SqlParameter resultUpdatedOnParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(GenreDto.Id), genre.Id.AsDbValue()),
            new SqlParameter(nameof(GenreDto.Name), genre.Name.AsDbValue()),
            new SqlParameter(nameof(GenreDto.Description), genre.Description.AsDbValue()),
            new SqlParameter(nameof(GenreDto.SystemEntity), genre.SystemEntity.AsDbValue()),
            new SqlParameter(nameof(GenreDto.Enabled), genre.Enabled.AsDbValue()),
            new SqlParameter(nameof(GenreDto.GenreRelationships), SqlDbType.Structured) { TypeName = "[dbo].[GenreRelationship]", Value = genreRelationshipsDataTable },
            resultIdParameter = new SqlParameter($"Result{nameof(GenreDto.Id)}", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output },
            resultCreatedOnParameter = new SqlParameter($"Result{nameof(GenreDto.CreatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
            resultUpdatedOnParameter = new SqlParameter($"Result{nameof(GenreDto.UpdatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_CreateGenre]
                @{nameof(GenreDto.Id)},
                @{nameof(GenreDto.Name)},
                @{nameof(GenreDto.Description)},
                @{nameof(GenreDto.SystemEntity)},
                @{nameof(GenreDto.Enabled)},
                @{nameof(GenreDto.GenreRelationships)},
                @{resultIdParameter.ParameterName} OUTPUT,
                @{resultCreatedOnParameter.ParameterName} OUTPUT,
                @{resultUpdatedOnParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        genre.Id = (Guid)resultIdParameter.Value;
        genre.CreatedOn = (DateTimeOffset)resultCreatedOnParameter.Value;
        genre.UpdatedOn = (DateTimeOffset)resultUpdatedOnParameter.Value;

        return genre;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateGenreAsync(GenreDto genre)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SetGenreRelationshipOrders(genre.GenreRelationships);

        using var genreRelationshipsDataTable = new DataTable();
        genreRelationshipsDataTable.Columns.Add(nameof(GenreRelationshipDto.GenreId), typeof(Guid));
        genreRelationshipsDataTable.Columns.Add(nameof(GenreRelationshipDto.DependentGenreId), typeof(Guid));
        genreRelationshipsDataTable.Columns.Add(nameof(GenreRelationshipDto.Name), typeof(string));
        genreRelationshipsDataTable.Columns.Add(nameof(GenreRelationshipDto.Description), typeof(string));
        genreRelationshipsDataTable.Columns.Add(nameof(GenreRelationshipDto.Order), typeof(int));
        foreach (GenreRelationshipDto genreRelationship in genre.GenreRelationships)
        {
            genreRelationshipsDataTable.Rows.Add(
                genreRelationship.GenreId.AsDbValue(),
                genreRelationship.DependentGenreId.AsDbValue(),
                genreRelationship.Name.AsDbValue(),
                genreRelationship.Description.AsDbValue(),
                genreRelationship.Order.AsDbValue());
        }

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(GenreDto.Id), genre.Id.AsDbValue()),
            new SqlParameter(nameof(GenreDto.Name), genre.Name.AsDbValue()),
            new SqlParameter(nameof(GenreDto.Description), genre.Description.AsDbValue()),
            new SqlParameter(nameof(GenreDto.SystemEntity), genre.SystemEntity.AsDbValue()),
            new SqlParameter(nameof(GenreDto.Enabled), genre.Enabled.AsDbValue()),
            new SqlParameter(nameof(GenreDto.GenreRelationships), SqlDbType.Structured) { TypeName = "[dbo].[GenreRelationship]", Value = genreRelationshipsDataTable },
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateGenre]
                @{nameof(GenreDto.Id)},
                @{nameof(GenreDto.Name)},
                @{nameof(GenreDto.Description)},
                @{nameof(GenreDto.SystemEntity)},
                @{nameof(GenreDto.Enabled)},
                @{nameof(GenreDto.GenreRelationships)},
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteGenreAsync(Guid genreId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultRowsDeletedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(GenreDto.Id), genreId.AsDbValue()),
            resultRowsDeletedParameter = new SqlParameter("ResultRowsDeleted", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_DeleteGenre]
                @{nameof(GenreDto.Id)},
                @{resultRowsDeletedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsDeleted = (int)resultRowsDeletedParameter.Value;
        return rowsDeleted > 0;
    }

    private static void OrderGenreRelationships(GenreDto genre)
    {
        genre.GenreRelationships = genre.GenreRelationships
            .OrderBy(genreRelationship => genreRelationship.Order)
            .ToList();
    }

    private static void SetGenreRelationshipOrders(IEnumerable<GenreRelationshipDto> genreRelationships)
    {
        var i = 0;
        foreach (GenreRelationshipDto genreRelationship in genreRelationships)
        {
            genreRelationship.Order = i++;
        }
    }
}
