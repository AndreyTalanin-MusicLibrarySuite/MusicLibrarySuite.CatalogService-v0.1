using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services;

/// <summary>
/// Represents a product service.
/// </summary>
public class ProductService : IProductService
{
    /// <inheritdoc />
    public Task<Product?> GetProductAsync(Guid productId) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Product[]> GetProductsAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Product[]> GetProductsAsync(IEnumerable<Guid> productIds) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<ProductPageResponse> GetProductsAsync(ProductRequest productRequest) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Product> CreateProductAsync(Product product) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> UpdateProductAsync(Product product) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> DeleteProductAsync(Guid productId) => throw new NotImplementedException();
}
