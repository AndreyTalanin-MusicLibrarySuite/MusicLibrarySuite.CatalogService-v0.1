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
/// Represents a SQL-Server-specific implementation of the product repository.
/// </summary>
public class SqlServerProductRepository : IProductRepository
{
    private readonly IDbContextFactory<CatalogServiceDbContext> m_contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerProductRepository" /> type using the specified services.
    /// </summary>
    /// <param name="contextFactory">The database context factory.</param>
    public SqlServerProductRepository(IDbContextFactory<CatalogServiceDbContext> contextFactory)
    {
        m_contextFactory = contextFactory;
    }

    /// <inheritdoc />
    public async Task<ProductDto?> GetProductAsync(Guid productId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        var productIdParameter = new SqlParameter("ProductId", productId.AsDbValue());

        var query = $"SELECT * FROM [dbo].[ufn_GetProduct] (@{productIdParameter.ParameterName})";

        ProductDto? product = await context.Products.FromSqlRaw(query, productIdParameter).AsNoTracking()
            .Include(product => product.ProductRelationships)
            .ThenInclude(productRelationship => productRelationship.DependentProduct)
            .FirstOrDefaultAsync();

        if (product is not null)
        {
            OrderProductRelationships(product);
        }

        return product;
    }

    /// <inheritdoc />
    public async Task<ProductDto[]> GetProductsAsync()
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ProductDto[] products = await context.Products.AsNoTracking()
            .Include(product => product.ProductRelationships)
            .ThenInclude(productRelationship => productRelationship.DependentProduct)
            .ToArrayAsync();

        foreach (ProductDto product in products)
        {
            OrderProductRelationships(product);
        }

        return products;
    }

    /// <inheritdoc />
    public async Task<ProductDto[]> GetProductsAsync(IEnumerable<Guid> productIds)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        using var productIdsDataTable = new DataTable();
        productIdsDataTable.Columns.Add("Value", typeof(Guid));
        foreach (Guid productId in productIds)
            productIdsDataTable.Rows.Add(productId.AsDbValue());

        var productIdsParameter = new SqlParameter("ProductIds", SqlDbType.Structured) { TypeName = "[dbo].[GuidArray]", Value = productIdsDataTable };

        var query = $"SELECT * FROM [dbo].[ufn_GetProducts] (@{productIdsParameter.ParameterName})";

        ProductDto[] products = await context.Products.FromSqlRaw(query, productIdsParameter).AsNoTracking()
            .Include(product => product.ProductRelationships)
            .ThenInclude(productRelationship => productRelationship.DependentProduct)
            .ToArrayAsync();

        foreach (ProductDto product in products)
        {
            OrderProductRelationships(product);
        }

        return products;
    }

    /// <inheritdoc />
    public async Task<ProductDto[]> GetProductsAsync(EntityCollectionProcessor<ProductDto> collectionProcessor)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        ProductDto[] products = await collectionProcessor(context.Products.AsNoTracking())
            .Include(product => product.ProductRelationships)
            .ThenInclude(productRelationship => productRelationship.DependentProduct)
            .ToArrayAsync();

        foreach (ProductDto product in products)
        {
            OrderProductRelationships(product);
        }

        return products;
    }

    /// <inheritdoc />
    public async Task<PageResponseDto<ProductDto>> GetProductsAsync(ProductPageRequestDto productPageRequest)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<ProductDto> baseCollection = context.Products.AsNoTracking();

        if (productPageRequest.Title is not null)
            baseCollection = baseCollection.Where(product => product.Title.Contains(productPageRequest.Title));

        if (productPageRequest.Enabled is not null)
            baseCollection = baseCollection.Where(product => product.Enabled == (bool)productPageRequest.Enabled);

        var totalCount = await baseCollection.CountAsync();
        List<ProductDto> products = await baseCollection
            .Include(product => product.ProductRelationships)
            .ThenInclude(productRelationship => productRelationship.DependentProduct)
            .OrderByDescending(product => product.SystemEntity)
            .ThenBy(product => product.Title)
            .Skip(productPageRequest.PageSize * productPageRequest.PageIndex)
            .Take(productPageRequest.PageSize)
            .ToListAsync();

        foreach (ProductDto product in products)
        {
            OrderProductRelationships(product);
        }

        return new PageResponseDto<ProductDto>()
        {
            PageSize = productPageRequest.PageSize,
            PageIndex = productPageRequest.PageIndex,
            TotalCount = totalCount,
            Items = products,
        };
    }

    /// <inheritdoc />
    public async Task<ProductRelationshipDto[]> GetProductRelationshipsAsync(Guid productId, bool includeReverseRelationships = false)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        IQueryable<ProductRelationshipDto> baseCollection = context.ProductRelationships.AsNoTracking()
            .Include(productRelationship => productRelationship.Product)
            .Include(productRelationship => productRelationship.DependentProduct);

        ProductRelationshipDto[] productRelationships = await baseCollection
            .Where(productRelationship => productRelationship.ProductId == productId)
            .OrderBy(productRelationship => productRelationship.Order)
            .ToArrayAsync();

        if (includeReverseRelationships)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            ProductRelationshipDto[] reverseRelationships = await baseCollection
                .Where(productRelationship => productRelationship.DependentProductId == productId)
                .OrderBy(productRelationship => productRelationship.Product.Title)
                .ToArrayAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            productRelationships = productRelationships.Concat(reverseRelationships).ToArray();
        }

        return productRelationships;
    }

    /// <inheritdoc />
    public async Task<ProductDto> CreateProductAsync(ProductDto product)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SetProductRelationshipOrders(product.ProductRelationships);

        using var productRelationshipsDataTable = new DataTable();
        productRelationshipsDataTable.Columns.Add(nameof(ProductRelationshipDto.ProductId), typeof(Guid));
        productRelationshipsDataTable.Columns.Add(nameof(ProductRelationshipDto.DependentProductId), typeof(Guid));
        productRelationshipsDataTable.Columns.Add(nameof(ProductRelationshipDto.Name), typeof(string));
        productRelationshipsDataTable.Columns.Add(nameof(ProductRelationshipDto.Description), typeof(string));
        productRelationshipsDataTable.Columns.Add(nameof(ProductRelationshipDto.Order), typeof(int));
        foreach (ProductRelationshipDto productRelationship in product.ProductRelationships)
        {
            productRelationshipsDataTable.Rows.Add(
                productRelationship.ProductId.AsDbValue(),
                productRelationship.DependentProductId.AsDbValue(),
                productRelationship.Name.AsDbValue(),
                productRelationship.Description.AsDbValue(),
                productRelationship.Order.AsDbValue());
        }

        SqlParameter resultIdParameter;
        SqlParameter resultCreatedOnParameter;
        SqlParameter resultUpdatedOnParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ProductDto.Id), product.Id.AsDbValue()),
            new SqlParameter(nameof(ProductDto.Title), product.Title.AsDbValue()),
            new SqlParameter(nameof(ProductDto.Description), product.Description.AsDbValue()),
            new SqlParameter(nameof(ProductDto.DisambiguationText), product.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(ProductDto.ReleasedOn), product.ReleasedOn.AsDbValue()),
            new SqlParameter(nameof(ProductDto.ReleasedOnYearOnly), product.ReleasedOnYearOnly.AsDbValue()),
            new SqlParameter(nameof(ProductDto.SystemEntity), product.SystemEntity.AsDbValue()),
            new SqlParameter(nameof(ProductDto.Enabled), product.Enabled.AsDbValue()),
            new SqlParameter(nameof(ProductDto.ProductRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ProductRelationship]", Value = productRelationshipsDataTable },
            resultIdParameter = new SqlParameter($"Result{nameof(ProductDto.Id)}", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output },
            resultCreatedOnParameter = new SqlParameter($"Result{nameof(ProductDto.CreatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
            resultUpdatedOnParameter = new SqlParameter($"Result{nameof(ProductDto.UpdatedOn)}", SqlDbType.DateTimeOffset) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_CreateProduct]
                @{nameof(ProductDto.Id)},
                @{nameof(ProductDto.Title)},
                @{nameof(ProductDto.Description)},
                @{nameof(ProductDto.DisambiguationText)},
                @{nameof(ProductDto.ReleasedOn)},
                @{nameof(ProductDto.ReleasedOnYearOnly)},
                @{nameof(ProductDto.SystemEntity)},
                @{nameof(ProductDto.Enabled)},
                @{nameof(ProductDto.ProductRelationships)},
                @{resultIdParameter.ParameterName} OUTPUT,
                @{resultCreatedOnParameter.ParameterName} OUTPUT,
                @{resultUpdatedOnParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        product.Id = (Guid)resultIdParameter.Value;
        product.CreatedOn = (DateTimeOffset)resultCreatedOnParameter.Value;
        product.UpdatedOn = (DateTimeOffset)resultUpdatedOnParameter.Value;

        return product;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateProductAsync(ProductDto product)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SetProductRelationshipOrders(product.ProductRelationships);

        using var productRelationshipsDataTable = new DataTable();
        productRelationshipsDataTable.Columns.Add(nameof(ProductRelationshipDto.ProductId), typeof(Guid));
        productRelationshipsDataTable.Columns.Add(nameof(ProductRelationshipDto.DependentProductId), typeof(Guid));
        productRelationshipsDataTable.Columns.Add(nameof(ProductRelationshipDto.Name), typeof(string));
        productRelationshipsDataTable.Columns.Add(nameof(ProductRelationshipDto.Description), typeof(string));
        productRelationshipsDataTable.Columns.Add(nameof(ProductRelationshipDto.Order), typeof(int));
        foreach (ProductRelationshipDto productRelationship in product.ProductRelationships)
        {
            productRelationshipsDataTable.Rows.Add(
                productRelationship.ProductId.AsDbValue(),
                productRelationship.DependentProductId.AsDbValue(),
                productRelationship.Name.AsDbValue(),
                productRelationship.Description.AsDbValue(),
                productRelationship.Order.AsDbValue());
        }

        SqlParameter resultRowsUpdatedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ProductDto.Id), product.Id.AsDbValue()),
            new SqlParameter(nameof(ProductDto.Title), product.Title.AsDbValue()),
            new SqlParameter(nameof(ProductDto.Description), product.Description.AsDbValue()),
            new SqlParameter(nameof(ProductDto.DisambiguationText), product.DisambiguationText.AsDbValue()),
            new SqlParameter(nameof(ProductDto.ReleasedOn), product.ReleasedOn.AsDbValue()),
            new SqlParameter(nameof(ProductDto.ReleasedOnYearOnly), product.ReleasedOnYearOnly.AsDbValue()),
            new SqlParameter(nameof(ProductDto.SystemEntity), product.SystemEntity.AsDbValue()),
            new SqlParameter(nameof(ProductDto.Enabled), product.Enabled.AsDbValue()),
            new SqlParameter(nameof(ProductDto.ProductRelationships), SqlDbType.Structured) { TypeName = "[dbo].[ProductRelationship]", Value = productRelationshipsDataTable },
            resultRowsUpdatedParameter = new SqlParameter("ResultRowsUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_UpdateProduct]
                @{nameof(ProductDto.Id)},
                @{nameof(ProductDto.Title)},
                @{nameof(ProductDto.Description)},
                @{nameof(ProductDto.DisambiguationText)},
                @{nameof(ProductDto.ReleasedOn)},
                @{nameof(ProductDto.ReleasedOnYearOnly)},
                @{nameof(ProductDto.SystemEntity)},
                @{nameof(ProductDto.Enabled)},
                @{nameof(ProductDto.ProductRelationships)},
                @{resultRowsUpdatedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsUpdated = (int)resultRowsUpdatedParameter.Value;
        return rowsUpdated > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        using CatalogServiceDbContext context = m_contextFactory.CreateDbContext();

        SqlParameter resultRowsDeletedParameter;
        var parameters = new SqlParameter[]
        {
            new SqlParameter(nameof(ProductDto.Id), productId.AsDbValue()),
            resultRowsDeletedParameter = new SqlParameter("ResultRowsDeleted", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        var query = @$"
            EXEC [dbo].[sp_DeleteProduct]
                @{nameof(ProductDto.Id)},
                @{resultRowsDeletedParameter.ParameterName} OUTPUT;";

        await context.Database.ExecuteSqlRawAsync(query, parameters);

        var rowsDeleted = (int)resultRowsDeletedParameter.Value;
        return rowsDeleted > 0;
    }

    private static void OrderProductRelationships(ProductDto product)
    {
        product.ProductRelationships = product.ProductRelationships
            .OrderBy(productRelationship => productRelationship.Order)
            .ToList();
    }

    private static void SetProductRelationshipOrders(IEnumerable<ProductRelationshipDto> productRelationships)
    {
        var i = 0;
        foreach (ProductRelationshipDto productRelationship in productRelationships)
        {
            productRelationship.Order = i++;
        }
    }
}
