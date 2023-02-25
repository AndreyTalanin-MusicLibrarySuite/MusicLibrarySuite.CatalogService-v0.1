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
/// Represents a SQL-Server-specific implementation of the work repository.
/// </summary>
public class SqlServerWorkRepository : IWorkRepository
{
    private readonly IDbContextFactory<CatalogServiceDbContext> m_contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerWorkRepository" /> type using the specified services.
    /// </summary>
    /// <param name="contextFactory">The database context factory.</param>
    public SqlServerWorkRepository(IDbContextFactory<CatalogServiceDbContext> contextFactory)
    {
        m_contextFactory = contextFactory;
    }

    /// <inheritdoc />
    public async Task<WorkDto?> GetWorkAsync(Guid workId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var workIdParameter = new SqlParameter("WorkId", workId.AsDbValue());

        var query = $"SELECT * FROM [dbo].[ufn_GetWork] (@{workIdParameter.ParameterName})";

        WorkDto? work = await context.Works
            .FromSqlRaw(query, workIdParameter)
            .Include(work => work.WorkRelationships)
            .ThenInclude(workRelationship => workRelationship.DependentWork)
            .Include(work => work.WorkToProductRelationships)
            .ThenInclude(workToProductRelationship => workToProductRelationship.Product)
            .Include(work => work.WorkArtists)
            .ThenInclude(workArtist => workArtist.Artist)
            .Include(work => work.WorkFeaturedArtists)
            .ThenInclude(workFeaturedArtist => workFeaturedArtist.Artist)
            .Include(work => work.WorkPerformers)
            .ThenInclude(workPerformer => workPerformer.Artist)
            .Include(work => work.WorkComposers)
            .ThenInclude(workComposer => workComposer.Artist)
            .Include(work => work.WorkGenres)
            .ThenInclude(workGenre => workGenre.Genre)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync();

        if (work is not null)
        {
            OrderWorkRelationships(work);
            OrderWorkToProductRelationships(work);
            OrderWorkArtists(work);
            OrderWorkFeaturedArtists(work);
            OrderWorkPerformers(work);
            OrderWorkComposers(work);
            OrderWorkGenres(work);
        }

        return work;
    }

    /// <inheritdoc />
    public async Task<WorkDto[]> GetWorksAsync()
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        WorkDto[] works = await context.Works
            .AsNoTracking()
            .ToArrayAsync();

        return works;
    }

    /// <inheritdoc />
    public async Task<WorkDto[]> GetWorksAsync(IEnumerable<Guid> workIds)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        using var workIdsDataTable = new DataTable();
        workIdsDataTable.Columns.Add("Value", typeof(Guid));
        foreach (Guid workId in workIds)
            workIdsDataTable.Rows.Add(workId.AsDbValue());

        var workIdsParameter = new SqlParameter("WorkIds", SqlDbType.Structured) { TypeName = "[dbo].[GuidArray]", Value = workIdsDataTable };

        var query = $"SELECT * FROM [dbo].[ufn_GetWorks] (@{workIdsParameter.ParameterName})";

        WorkDto[] works = await context.Works
            .FromSqlRaw(query, workIdsParameter)
            .AsNoTracking()
            .ToArrayAsync();

        return works;
    }

    /// <inheritdoc />
    public async Task<WorkDto[]> GetWorksAsync(EntityCollectionProcessor<WorkDto> collectionProcessor)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        WorkDto[] works = await collectionProcessor(context.Works)
            .AsNoTracking()
            .ToArrayAsync();

        return works;
    }

    /// <inheritdoc />
    public async Task<PageResponseDto<WorkDto>> GetWorksAsync(WorkPageRequestDto workPageRequest)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<WorkDto> baseCollection = context.Works;

        if (workPageRequest.Title is not null)
            baseCollection = baseCollection.Where(work => work.Title.Contains(workPageRequest.Title));

        if (workPageRequest.Enabled is not null)
            baseCollection = baseCollection.Where(work => work.Enabled == (bool)workPageRequest.Enabled);

        var totalCount = await baseCollection.CountAsync();
        List<WorkDto> works = await baseCollection
            .OrderByDescending(work => work.SystemEntity)
            .ThenBy(work => work.Title)
            .Skip(workPageRequest.PageSize * workPageRequest.PageIndex)
            .Take(workPageRequest.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PageResponseDto<WorkDto>()
        {
            PageSize = workPageRequest.PageSize,
            PageIndex = workPageRequest.PageIndex,
            TotalCount = totalCount,
            Items = works,
        };
    }

    /// <inheritdoc/>
    public async Task<WorkRelationshipDto[]> GetWorkRelationshipsAsync(Guid workId, bool includeReverseRelationships = false)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<WorkRelationshipDto> baseCollection = context.WorkRelationships.AsNoTracking()
            .Include(workRelationship => workRelationship.Work)
            .Include(workRelationship => workRelationship.DependentWork);

        WorkRelationshipDto[] workRelationships = await baseCollection
            .Where(workRelationship => workRelationship.WorkId == workId)
            .OrderBy(workRelationship => workRelationship.Order)
            .ToArrayAsync();

        if (includeReverseRelationships)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            WorkRelationshipDto[] reverseRelationships = await baseCollection
                .Where(workRelationship => workRelationship.DependentWorkId == workId)
                .OrderBy(workRelationship => workRelationship.Work.Title)
                .ToArrayAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            workRelationships = workRelationships.Concat(reverseRelationships).ToArray();
        }

        return workRelationships;
    }

    /// <inheritdoc />
    public async Task<WorkToProductRelationshipDto[]> GetWorkToProductRelationshipsAsync(Guid workId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        WorkToProductRelationshipDto[] workToProductRelationships = await context.WorkToProductRelationships.AsNoTracking()
            .Include(workToProductRelationship => workToProductRelationship.Work)
            .Include(workToProductRelationship => workToProductRelationship.Product)
            .Where(workToProductRelationship => workToProductRelationship.WorkId == workId)
            .OrderBy(workToProductRelationship => workToProductRelationship.Order)
            .ToArrayAsync();

        return workToProductRelationships;
    }

    /// <inheritdoc/>
    public async Task<WorkToProductRelationshipDto[]> GetWorkToProductRelationshipsByProductAsync(Guid productId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        WorkToProductRelationshipDto[] workToProductRelationships = await context.WorkToProductRelationships.AsNoTracking()
            .Include(workToProductRelationship => workToProductRelationship.Work)
            .Include(workToProductRelationship => workToProductRelationship.Product)
            .Where(workToProductRelationship => workToProductRelationship.ProductId == productId)
            .OrderBy(workToProductRelationship => workToProductRelationship.ReferenceOrder)
            .ToArrayAsync();

        return workToProductRelationships;
    }

    /// <inheritdoc />
    public async Task<WorkDto> CreateWorkAsync(WorkDto work)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SetWorkRelationshipOrders(work.WorkRelationships);
        SetWorkToProductRelationshipOrders(work.WorkToProductRelationships);
        SetWorkArtistOrders(work.WorkArtists);
        SetWorkFeaturedArtistOrders(work.WorkFeaturedArtists);
        SetWorkPerformerOrders(work.WorkPerformers);
        SetWorkComposerOrders(work.WorkComposers);
        SetWorkGenreOrders(work.WorkGenres);

        using var workRelationshipsDataTable = new DataTable();
        workRelationshipsDataTable.Columns.Add(nameof(WorkRelationshipDto.WorkId), typeof(Guid));
        workRelationshipsDataTable.Columns.Add(nameof(WorkRelationshipDto.DependentWorkId), typeof(Guid));
        workRelationshipsDataTable.Columns.Add(nameof(WorkRelationshipDto.Name), typeof(string));
        workRelationshipsDataTable.Columns.Add(nameof(WorkRelationshipDto.Description), typeof(string));
        workRelationshipsDataTable.Columns.Add(nameof(WorkRelationshipDto.Order), typeof(int));
        foreach (WorkRelationshipDto workRelationship in work.WorkRelationships)
        {
            workRelationshipsDataTable.Rows.Add(
                workRelationship.WorkId.AsDbValue(),
                workRelationship.DependentWorkId.AsDbValue(),
                workRelationship.Name.AsDbValue(),
                workRelationship.Description.AsDbValue(),
                workRelationship.Order.AsDbValue());
        }

        using var workToProductRelationshipsDataTable = new DataTable();
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.WorkId), typeof(Guid));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.ProductId), typeof(Guid));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.Name), typeof(string));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.Description), typeof(string));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.Order), typeof(int));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.ReferenceOrder), typeof(int));
        foreach (WorkToProductRelationshipDto workToProductRelationship in work.WorkToProductRelationships)
        {
            workToProductRelationshipsDataTable.Rows.Add(
                workToProductRelationship.WorkId.AsDbValue(),
                workToProductRelationship.ProductId.AsDbValue(),
                workToProductRelationship.Name.AsDbValue(),
                workToProductRelationship.Description.AsDbValue(),
                workToProductRelationship.Order.AsDbValue(),
                workToProductRelationship.ReferenceOrder.AsDbValue());
        }

        using var workArtistsDataTable = new DataTable();
        workArtistsDataTable.Columns.Add(nameof(WorkArtistDto.WorkId), typeof(Guid));
        workArtistsDataTable.Columns.Add(nameof(WorkArtistDto.ArtistId), typeof(Guid));
        workArtistsDataTable.Columns.Add(nameof(WorkArtistDto.Order), typeof(int));
        foreach (WorkArtistDto workArtist in work.WorkArtists)
        {
            workArtistsDataTable.Rows.Add(
                workArtist.WorkId.AsDbValue(),
                workArtist.ArtistId.AsDbValue(),
                workArtist.Order.AsDbValue());
        }

        using var workFeaturedArtistsDataTable = new DataTable();
        workFeaturedArtistsDataTable.Columns.Add(nameof(WorkFeaturedArtistDto.WorkId), typeof(Guid));
        workFeaturedArtistsDataTable.Columns.Add(nameof(WorkFeaturedArtistDto.ArtistId), typeof(Guid));
        workFeaturedArtistsDataTable.Columns.Add(nameof(WorkFeaturedArtistDto.Order), typeof(int));
        foreach (WorkFeaturedArtistDto workFeaturedArtist in work.WorkFeaturedArtists)
        {
            workFeaturedArtistsDataTable.Rows.Add(
                workFeaturedArtist.WorkId.AsDbValue(),
                workFeaturedArtist.ArtistId.AsDbValue(),
                workFeaturedArtist.Order.AsDbValue());
        }

        using var workPerformersDataTable = new DataTable();
        workPerformersDataTable.Columns.Add(nameof(WorkPerformerDto.WorkId), typeof(Guid));
        workPerformersDataTable.Columns.Add(nameof(WorkPerformerDto.ArtistId), typeof(Guid));
        workPerformersDataTable.Columns.Add(nameof(WorkPerformerDto.Order), typeof(int));
        foreach (WorkPerformerDto workPerformer in work.WorkPerformers)
        {
            workPerformersDataTable.Rows.Add(
                workPerformer.WorkId.AsDbValue(),
                workPerformer.ArtistId.AsDbValue(),
                workPerformer.Order.AsDbValue());
        }

        using var workComposersDataTable = new DataTable();
        workComposersDataTable.Columns.Add(nameof(WorkComposerDto.WorkId), typeof(Guid));
        workComposersDataTable.Columns.Add(nameof(WorkComposerDto.ArtistId), typeof(Guid));
        workComposersDataTable.Columns.Add(nameof(WorkComposerDto.Order), typeof(int));
        foreach (WorkComposerDto workComposer in work.WorkComposers)
        {
            workComposersDataTable.Rows.Add(
                workComposer.WorkId.AsDbValue(),
                workComposer.ArtistId.AsDbValue(),
                workComposer.Order.AsDbValue());
        }

        using var workGenresDataTable = new DataTable();
        workGenresDataTable.Columns.Add(nameof(WorkGenreDto.WorkId), typeof(Guid));
        workGenresDataTable.Columns.Add(nameof(WorkGenreDto.GenreId), typeof(Guid));
        workGenresDataTable.Columns.Add(nameof(WorkGenreDto.Order), typeof(int));
        foreach (WorkGenreDto workGenre in work.WorkGenres)
        {
            workGenresDataTable.Rows.Add(
                workGenre.WorkId.AsDbValue(),
                workGenre.GenreId.AsDbValue(),
                workGenre.Order.AsDbValue());
        }

        SqlParameter resultIdParameter;
        SqlParameter resultCreatedOnParameter;
        SqlParameter resultUpdatedOnParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(WorkDto.Id), work.Id.AsDbValue()),
            new SqlParameter(nameof(WorkDto.Title), work.Title.AsDbValue()),
            new SqlParameter(nameof(WorkDto.Description), work.Description.AsDbValue()),
            new SqlParameter(nameof(WorkDto.DisambiguationText), work.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(WorkDto.InternationalStandardMusicalWorkCode), work.InternationalStandardMusicalWorkCode.AsDbValue()),
            new SqlParameter(nameof(WorkDto.ReleasedOn), work.ReleasedOn.AsDbValue()),
            new SqlParameter(nameof(WorkDto.ReleasedOnYearOnly), work.ReleasedOnYearOnly.AsDbValue()),
            new SqlParameter(nameof(WorkDto.SystemEntity), work.SystemEntity.AsDbValue()),
            new SqlParameter(nameof(WorkDto.Enabled), work.Enabled.AsDbValue()),
            new SqlParameter(nameof(WorkDto.WorkRelationships), SqlDbType.Structured) { TypeName = "[dbo].[WorkRelationship]", Value = workRelationshipsDataTable },
            new SqlParameter(nameof(WorkDto.WorkToProductRelationships), SqlDbType.Structured) { TypeName = "[dbo].[WorkToProductRelationship]", Value = workToProductRelationshipsDataTable },
            new SqlParameter(nameof(WorkDto.WorkArtists), SqlDbType.Structured) { TypeName = "[dbo].[WorkArtist]", Value = workArtistsDataTable },
            new SqlParameter(nameof(WorkDto.WorkFeaturedArtists), SqlDbType.Structured) { TypeName = "[dbo].[WorkFeaturedArtist]", Value = workFeaturedArtistsDataTable },
            new SqlParameter(nameof(WorkDto.WorkPerformers), SqlDbType.Structured) { TypeName = "[dbo].[WorkPerformer]", Value = workPerformersDataTable },
            new SqlParameter(nameof(WorkDto.WorkComposers), SqlDbType.Structured) { TypeName = "[dbo].[WorkComposer]", Value = workComposersDataTable },
            new SqlParameter(nameof(WorkDto.WorkGenres), SqlDbType.Structured) { TypeName = "[dbo].[WorkGenre]", Value = workGenresDataTable },
            resultIdParameter = new SqlParameter($"Result{nameof(WorkDto.Id)}", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output },
            resultCreatedOnParameter = new SqlParameter($"Result{nameof(WorkDto.CreatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
            resultUpdatedOnParameter = new SqlParameter($"Result{nameof(WorkDto.UpdatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_CreateWork]
                @{nameof(WorkDto.Id)},
                @{nameof(WorkDto.Title)},
                @{nameof(WorkDto.Description)},
                @{nameof(WorkDto.DisambiguationText)},
                @{nameof(WorkDto.InternationalStandardMusicalWorkCode)},
                @{nameof(WorkDto.ReleasedOn)},
                @{nameof(WorkDto.ReleasedOnYearOnly)},
                @{nameof(WorkDto.SystemEntity)},
                @{nameof(WorkDto.Enabled)},
                @{nameof(WorkDto.WorkRelationships)},
                @{nameof(WorkDto.WorkToProductRelationships)},
                @{nameof(WorkDto.WorkArtists)},
                @{nameof(WorkDto.WorkFeaturedArtists)},
                @{nameof(WorkDto.WorkPerformers)},
                @{nameof(WorkDto.WorkComposers)},
                @{nameof(WorkDto.WorkGenres)},
                @{resultIdParameter.ParameterName} OUTPUT,
                @{resultCreatedOnParameter.ParameterName} OUTPUT,
                @{resultUpdatedOnParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        work.Id = (Guid)resultIdParameter.Value;
        work.CreatedOn = (DateTimeOffset)resultCreatedOnParameter.Value;
        work.UpdatedOn = (DateTimeOffset)resultUpdatedOnParameter.Value;

        return work;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateWorkAsync(WorkDto work)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SetWorkRelationshipOrders(work.WorkRelationships);
        SetWorkToProductRelationshipOrders(work.WorkToProductRelationships);
        SetWorkArtistOrders(work.WorkArtists);
        SetWorkFeaturedArtistOrders(work.WorkFeaturedArtists);
        SetWorkPerformerOrders(work.WorkPerformers);
        SetWorkComposerOrders(work.WorkComposers);
        SetWorkGenreOrders(work.WorkGenres);

        using var workRelationshipsDataTable = new DataTable();
        workRelationshipsDataTable.Columns.Add(nameof(WorkRelationshipDto.WorkId), typeof(Guid));
        workRelationshipsDataTable.Columns.Add(nameof(WorkRelationshipDto.DependentWorkId), typeof(Guid));
        workRelationshipsDataTable.Columns.Add(nameof(WorkRelationshipDto.Name), typeof(string));
        workRelationshipsDataTable.Columns.Add(nameof(WorkRelationshipDto.Description), typeof(string));
        workRelationshipsDataTable.Columns.Add(nameof(WorkRelationshipDto.Order), typeof(int));
        foreach (WorkRelationshipDto workRelationship in work.WorkRelationships)
        {
            workRelationshipsDataTable.Rows.Add(
                workRelationship.WorkId.AsDbValue(),
                workRelationship.DependentWorkId.AsDbValue(),
                workRelationship.Name.AsDbValue(),
                workRelationship.Description.AsDbValue(),
                workRelationship.Order.AsDbValue());
        }

        using var workToProductRelationshipsDataTable = new DataTable();
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.WorkId), typeof(Guid));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.ProductId), typeof(Guid));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.Name), typeof(string));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.Description), typeof(string));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.Order), typeof(int));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.ReferenceOrder), typeof(int));
        foreach (WorkToProductRelationshipDto workToProductRelationship in work.WorkToProductRelationships)
        {
            workToProductRelationshipsDataTable.Rows.Add(
                workToProductRelationship.WorkId.AsDbValue(),
                workToProductRelationship.ProductId.AsDbValue(),
                workToProductRelationship.Name.AsDbValue(),
                workToProductRelationship.Description.AsDbValue(),
                workToProductRelationship.Order.AsDbValue(),
                workToProductRelationship.ReferenceOrder.AsDbValue());
        }

        using var workArtistsDataTable = new DataTable();
        workArtistsDataTable.Columns.Add(nameof(WorkArtistDto.WorkId), typeof(Guid));
        workArtistsDataTable.Columns.Add(nameof(WorkArtistDto.ArtistId), typeof(Guid));
        workArtistsDataTable.Columns.Add(nameof(WorkArtistDto.Order), typeof(int));
        foreach (WorkArtistDto workArtist in work.WorkArtists)
        {
            workArtistsDataTable.Rows.Add(
                workArtist.WorkId.AsDbValue(),
                workArtist.ArtistId.AsDbValue(),
                workArtist.Order.AsDbValue());
        }

        using var workFeaturedArtistsDataTable = new DataTable();
        workFeaturedArtistsDataTable.Columns.Add(nameof(WorkFeaturedArtistDto.WorkId), typeof(Guid));
        workFeaturedArtistsDataTable.Columns.Add(nameof(WorkFeaturedArtistDto.ArtistId), typeof(Guid));
        workFeaturedArtistsDataTable.Columns.Add(nameof(WorkFeaturedArtistDto.Order), typeof(int));
        foreach (WorkFeaturedArtistDto workFeaturedArtist in work.WorkFeaturedArtists)
        {
            workFeaturedArtistsDataTable.Rows.Add(
                workFeaturedArtist.WorkId.AsDbValue(),
                workFeaturedArtist.ArtistId.AsDbValue(),
                workFeaturedArtist.Order.AsDbValue());
        }

        using var workPerformersDataTable = new DataTable();
        workPerformersDataTable.Columns.Add(nameof(WorkPerformerDto.WorkId), typeof(Guid));
        workPerformersDataTable.Columns.Add(nameof(WorkPerformerDto.ArtistId), typeof(Guid));
        workPerformersDataTable.Columns.Add(nameof(WorkPerformerDto.Order), typeof(int));
        foreach (WorkPerformerDto workPerformer in work.WorkPerformers)
        {
            workPerformersDataTable.Rows.Add(
                workPerformer.WorkId.AsDbValue(),
                workPerformer.ArtistId.AsDbValue(),
                workPerformer.Order.AsDbValue());
        }

        using var workComposersDataTable = new DataTable();
        workComposersDataTable.Columns.Add(nameof(WorkComposerDto.WorkId), typeof(Guid));
        workComposersDataTable.Columns.Add(nameof(WorkComposerDto.ArtistId), typeof(Guid));
        workComposersDataTable.Columns.Add(nameof(WorkComposerDto.Order), typeof(int));
        foreach (WorkComposerDto workComposer in work.WorkComposers)
        {
            workComposersDataTable.Rows.Add(
                workComposer.WorkId.AsDbValue(),
                workComposer.ArtistId.AsDbValue(),
                workComposer.Order.AsDbValue());
        }

        using var workGenresDataTable = new DataTable();
        workGenresDataTable.Columns.Add(nameof(WorkGenreDto.WorkId), typeof(Guid));
        workGenresDataTable.Columns.Add(nameof(WorkGenreDto.GenreId), typeof(Guid));
        workGenresDataTable.Columns.Add(nameof(WorkGenreDto.Order), typeof(int));
        foreach (WorkGenreDto workGenre in work.WorkGenres)
        {
            workGenresDataTable.Rows.Add(
                workGenre.WorkId.AsDbValue(),
                workGenre.GenreId.AsDbValue(),
                workGenre.Order.AsDbValue());
        }

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(WorkDto.Id), work.Id.AsDbValue()),
            new SqlParameter(nameof(WorkDto.Title), work.Title.AsDbValue()),
            new SqlParameter(nameof(WorkDto.Description), work.Description.AsDbValue()),
            new SqlParameter(nameof(WorkDto.DisambiguationText), work.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(WorkDto.InternationalStandardMusicalWorkCode), work.InternationalStandardMusicalWorkCode.AsDbValue()),
            new SqlParameter(nameof(WorkDto.ReleasedOn), work.ReleasedOn.AsDbValue()),
            new SqlParameter(nameof(WorkDto.ReleasedOnYearOnly), work.ReleasedOnYearOnly.AsDbValue()),
            new SqlParameter(nameof(WorkDto.SystemEntity), work.SystemEntity.AsDbValue()),
            new SqlParameter(nameof(WorkDto.Enabled), work.Enabled.AsDbValue()),
            new SqlParameter(nameof(WorkDto.WorkRelationships), SqlDbType.Structured) { TypeName = "[dbo].[WorkRelationship]", Value = workRelationshipsDataTable },
            new SqlParameter(nameof(WorkDto.WorkToProductRelationships), SqlDbType.Structured) { TypeName = "[dbo].[WorkToProductRelationship]", Value = workToProductRelationshipsDataTable },
            new SqlParameter(nameof(WorkDto.WorkArtists), SqlDbType.Structured) { TypeName = "[dbo].[WorkArtist]", Value = workArtistsDataTable },
            new SqlParameter(nameof(WorkDto.WorkFeaturedArtists), SqlDbType.Structured) { TypeName = "[dbo].[WorkFeaturedArtist]", Value = workFeaturedArtistsDataTable },
            new SqlParameter(nameof(WorkDto.WorkPerformers), SqlDbType.Structured) { TypeName = "[dbo].[WorkPerformer]", Value = workPerformersDataTable },
            new SqlParameter(nameof(WorkDto.WorkComposers), SqlDbType.Structured) { TypeName = "[dbo].[WorkComposer]", Value = workComposersDataTable },
            new SqlParameter(nameof(WorkDto.WorkGenres), SqlDbType.Structured) { TypeName = "[dbo].[WorkGenre]", Value = workGenresDataTable },
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateWork]
                @{nameof(WorkDto.Id)},
                @{nameof(WorkDto.Title)},
                @{nameof(WorkDto.Description)},
                @{nameof(WorkDto.DisambiguationText)},
                @{nameof(WorkDto.InternationalStandardMusicalWorkCode)},
                @{nameof(WorkDto.ReleasedOn)},
                @{nameof(WorkDto.ReleasedOnYearOnly)},
                @{nameof(WorkDto.SystemEntity)},
                @{nameof(WorkDto.Enabled)},
                @{nameof(WorkDto.WorkRelationships)},
                @{nameof(WorkDto.WorkToProductRelationships)},
                @{nameof(WorkDto.WorkArtists)},
                @{nameof(WorkDto.WorkFeaturedArtists)},
                @{nameof(WorkDto.WorkPerformers)},
                @{nameof(WorkDto.WorkComposers)},
                @{nameof(WorkDto.WorkGenres)},
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated > 0;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateWorkToProductRelationshipsOrderAsync(WorkToProductRelationshipDto[] workToProductRelationships, bool useReferenceOrder)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var i = 0;
        foreach (WorkToProductRelationshipDto workToProductRelationship in workToProductRelationships)
        {
            if (!useReferenceOrder)
                workToProductRelationship.Order = i++;
            else
                workToProductRelationship.ReferenceOrder = i++;
        }

        using var workToProductRelationshipsDataTable = new DataTable();
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.WorkId), typeof(Guid));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.ProductId), typeof(Guid));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.Name), typeof(string));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.Description), typeof(string));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.Order), typeof(int));
        workToProductRelationshipsDataTable.Columns.Add(nameof(WorkToProductRelationshipDto.ReferenceOrder), typeof(int));
        foreach (WorkToProductRelationshipDto workToProductRelationship in workToProductRelationships)
        {
            workToProductRelationshipsDataTable.Rows.Add(
                workToProductRelationship.WorkId.AsDbValue(),
                workToProductRelationship.ProductId.AsDbValue(),
                workToProductRelationship.Name.AsDbValue(),
                workToProductRelationship.Description.AsDbValue(),
                workToProductRelationship.Order.AsDbValue(),
                workToProductRelationship.ReferenceOrder.AsDbValue());
        }

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter("UseReferenceOrder", useReferenceOrder.AsDbValue()),
            new SqlParameter("WorkToProductRelationships", SqlDbType.Structured) { TypeName = "[dbo].[WorkToProductRelationship]", Value = workToProductRelationshipsDataTable },
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateWorkToProductRelationshipsOrder]
                @UseReferenceOrder,
                @WorkToProductRelationships,
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteWorkAsync(Guid workId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultRowsDeletedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(WorkDto.Id), workId.AsDbValue()),
            resultRowsDeletedParameter = new SqlParameter("ResultRowsDeleted", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_DeleteWork]
                @{nameof(WorkDto.Id)},
                @{resultRowsDeletedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsDeleted = (int)resultRowsDeletedParameter.Value;
        return rowsDeleted > 0;
    }

    private static void OrderWorkRelationships(WorkDto work)
    {
        work.WorkRelationships = work.WorkRelationships
            .OrderBy(workRelationship => workRelationship.Order)
            .ToList();
    }

    private static void OrderWorkToProductRelationships(WorkDto work)
    {
        work.WorkToProductRelationships = work.WorkToProductRelationships
            .OrderBy(workToProductRelationship => workToProductRelationship.Order)
            .ToList();
    }

    private static void OrderWorkArtists(WorkDto work)
    {
        work.WorkArtists = work.WorkArtists
            .OrderBy(workArtist => workArtist.Order)
            .ToList();
    }

    private static void OrderWorkFeaturedArtists(WorkDto work)
    {
        work.WorkFeaturedArtists = work.WorkFeaturedArtists
            .OrderBy(workFeaturedArtist => workFeaturedArtist.Order)
            .ToList();
    }

    private static void OrderWorkPerformers(WorkDto work)
    {
        work.WorkPerformers = work.WorkPerformers
            .OrderBy(workPerformer => workPerformer.Order)
            .ToList();
    }

    private static void OrderWorkComposers(WorkDto work)
    {
        work.WorkComposers = work.WorkComposers
            .OrderBy(workComposer => workComposer.Order)
            .ToList();
    }

    private static void OrderWorkGenres(WorkDto work)
    {
        work.WorkGenres = work.WorkGenres
            .OrderBy(workGenre => workGenre.Order)
            .ToList();
    }

    private static void SetWorkRelationshipOrders(IEnumerable<WorkRelationshipDto> workRelationships)
    {
        var i = 0;
        foreach (WorkRelationshipDto workRelationship in workRelationships)
        {
            workRelationship.Order = i++;
        }
    }

    private static void SetWorkToProductRelationshipOrders(IEnumerable<WorkToProductRelationshipDto> workToProductRelationships)
    {
        var i = 0;
        foreach (WorkToProductRelationshipDto workToProductRelationship in workToProductRelationships)
        {
            workToProductRelationship.Order = i++;
        }
    }

    private static void SetWorkArtistOrders(IEnumerable<WorkArtistDto> workArtists)
    {
        var i = 0;
        foreach (WorkArtistDto workArtist in workArtists)
        {
            workArtist.Order = i++;
        }
    }

    private static void SetWorkFeaturedArtistOrders(IEnumerable<WorkFeaturedArtistDto> workFeaturedArtists)
    {
        var i = 0;
        foreach (WorkFeaturedArtistDto workFeaturedArtist in workFeaturedArtists)
        {
            workFeaturedArtist.Order = i++;
        }
    }

    private static void SetWorkPerformerOrders(IEnumerable<WorkPerformerDto> workPerformers)
    {
        var i = 0;
        foreach (WorkPerformerDto workPerformer in workPerformers)
        {
            workPerformer.Order = i++;
        }
    }

    private static void SetWorkComposerOrders(IEnumerable<WorkComposerDto> workComposers)
    {
        var i = 0;
        foreach (WorkComposerDto workComposer in workComposers)
        {
            workComposer.Order = i++;
        }
    }

    private static void SetWorkGenreOrders(IEnumerable<WorkGenreDto> workGenres)
    {
        var i = 0;
        foreach (WorkGenreDto workGenre in workGenres)
        {
            workGenre.Order = i++;
        }
    }
}
