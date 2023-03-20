using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services.Abstractions;

/// <summary>
/// Defines a set of members a product service should implement.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Asynchronously gets a product by its unique identifier.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the product found or <see langword="null" />.
    /// </returns>
    public Task<Product?> GetProductAsync(Guid productId);

    /// <summary>
    /// Asynchronously gets all products.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all products.
    /// </returns>
    public Task<Product[]> GetProductsAsync();

    /// <summary>
    /// Asynchronously gets products by a collection of unique identifiers.
    /// </summary>
    /// <param name="productIds">The collection of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found products.
    /// </returns>
    public Task<Product[]> GetProductsAsync(IEnumerable<Guid> productIds);

    /// <summary>
    /// Asynchronously gets products by a product page request.
    /// </summary>
    /// <param name="productPageRequest">The product page request configuration.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all products corresponding to the request configuration.
    /// </returns>
    public Task<ProductPageResponse> GetProductsAsync(ProductPageRequest productPageRequest);

    /// <summary>
    /// Asynchronously gets all product relationships by a product's unique identifier.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <param name="includeReverseRelationships">A boolean value specifying whether reverse relationships should be included.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all product relationships.
    /// </returns>
    public Task<ProductRelationship[]> GetProductRelationshipsAsync(Guid productId, bool includeReverseRelationships = false);

    /// <summary>
    /// Asynchronously creates a new product.
    /// </summary>
    /// <param name="product">The product to create.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created product with properties like <see cref="Product.Id" /> set.
    /// </returns>
    public Task<Product> CreateProductAsync(Product product);

    /// <summary>
    /// Asynchronously updates an existing product.
    /// </summary>
    /// <param name="product">The product to update.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the product was found and updated.
    /// </returns>
    public Task<bool> UpdateProductAsync(Product product);

    /// <summary>
    /// Asynchronously deletes an existing product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to delete.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the product was found and deleted.
    /// </returns>
    public Task<bool> DeleteProductAsync(Guid productId);
}
