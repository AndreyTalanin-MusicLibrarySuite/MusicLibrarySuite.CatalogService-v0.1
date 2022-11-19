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
/// Represents a SQL Server - specific implementation of the genre repository.
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

        return await context.Genres.FromSqlRaw(query, genreIdParameter).AsNoTracking()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<GenreDto[]> GetGenresAsync()
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        return await context.Genres.AsNoTracking()
            .ToArrayAsync();
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

        return await context.Genres.FromSqlRaw(query, genreIdsParameter).AsNoTracking()
            .ToArrayAsync();
    }

    /// <inheritdoc />
    public async Task<GenreDto[]> GetGenresAsync(EntityCollectionProcessor<GenreDto> collectionProcessor)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        return await collectionProcessor(context.Genres.AsNoTracking())
            .ToArrayAsync();
    }

    /// <inheritdoc />
    public async Task<PageResponseDto<GenreDto>> GetGenresAsync(GenreRequestDto genreRequest)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<GenreDto> baseCollection = context.Genres.AsNoTracking();

        if (genreRequest.Name is not null)
            baseCollection = baseCollection.Where(genre => genre.Name.Contains(genreRequest.Name));

        if (genreRequest.Enabled is not null)
            baseCollection = baseCollection.Where(genre => genre.Enabled == (bool)genreRequest.Enabled);

        var totalCount = await baseCollection.CountAsync();
        List<GenreDto> genres = await baseCollection
            .OrderByDescending(genre => genre.SystemEntity)
            .ThenBy(genre => genre.Name)
            .Skip(genreRequest.PageSize * genreRequest.PageIndex)
            .Take(genreRequest.PageSize)
            .ToListAsync();

        return new PageResponseDto<GenreDto>()
        {
            PageSize = genreRequest.PageSize,
            PageIndex = genreRequest.PageIndex,
            TotalCount = totalCount,
            Items = genres,
        };
    }

    /// <inheritdoc />
    public async Task<GenreDto> CreateGenreAsync(GenreDto genre)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

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

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(GenreDto.Id), genre.Id.AsDbValue()),
            new SqlParameter(nameof(GenreDto.Name), genre.Name.AsDbValue()),
            new SqlParameter(nameof(GenreDto.Description), genre.Description.AsDbValue()),
            new SqlParameter(nameof(GenreDto.SystemEntity), genre.SystemEntity.AsDbValue()),
            new SqlParameter(nameof(GenreDto.Enabled), genre.Enabled.AsDbValue()),
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateGenre]
                @{nameof(GenreDto.Id)},
                @{nameof(GenreDto.Name)},
                @{nameof(GenreDto.Description)},
                @{nameof(GenreDto.SystemEntity)},
                @{nameof(GenreDto.Enabled)},
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
}
