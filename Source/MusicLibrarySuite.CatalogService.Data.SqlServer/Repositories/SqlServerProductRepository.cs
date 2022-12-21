using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Data.Repositories;
using MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

namespace MusicLibrarySuite.CatalogService.Data.SqlServer.Repositories;

/// <summary>
/// Represents a SQL Server - specific implementation of the product repository.
/// </summary>
public class SqlServerProductRepository : IProductRepository
{
    /// <inheritdoc />
    public Task<ProductDto?> GetProductAsync(Guid productId) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ProductDto[]> GetProductsAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ProductDto[]> GetProductsAsync(IEnumerable<Guid> productIds) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ProductDto[]> GetProductsAsync(EntityCollectionProcessor<ProductDto> collectionProcessor) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<PageResponseDto<ProductDto>> GetProductsAsync(ProductRequestDto productRequest) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ProductDto> CreateProductAsync(ProductDto product) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> UpdateProductAsync(ProductDto product) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> DeleteProductAsync(Guid productId) => throw new NotImplementedException();
}
