using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;

/// <summary>
/// Defines a set of members a provider-specific product repository should implement.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Asynchronously gets a product by its unique identifier.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the product found or <see langword="null" />.
    /// </returns>
    public Task<ProductDto?> GetProductAsync(Guid productId);

    /// <summary>
    /// Asynchronously gets all products.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all products.
    /// </returns>
    public Task<ProductDto[]> GetProductsAsync();

    /// <summary>
    /// Asynchronously gets products by a collection of unique identifiers.
    /// </summary>
    /// <param name="productIds">The collection of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found products.
    /// </returns>
    public Task<ProductDto[]> GetProductsAsync(IEnumerable<Guid> productIds);

    /// <summary>
    /// Asynchronously gets products filtered by a collection processor.
    /// </summary>
    /// <param name="collectionProcessor">The collection processor to filter products.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found products.
    /// </returns>
    public Task<ProductDto[]> GetProductsAsync(EntityCollectionProcessor<ProductDto> collectionProcessor);

    /// <summary>
    /// Asynchronously gets products by a product page request.
    /// </summary>
    /// <param name="productRequest">The product page request configuration.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all products corresponding to the request configuration.
    /// </returns>
    public Task<PageResponseDto<ProductDto>> GetProductsAsync(ProductRequestDto productRequest);

    /// <summary>
    /// Asynchronously gets all product relationships by a product's unique identifier.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <param name="includeReverseRelationships">A boolean value specifying whether reverse relationships should be included.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all product relationships.
    /// </returns>
    public Task<ProductRelationshipDto[]> GetProductRelationshipsAsync(Guid productId, bool includeReverseRelationships = false);

    /// <summary>
    /// Asynchronously creates a new product.
    /// </summary>
    /// <param name="product">The product to create in the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created product with properties like <see cref="ProductDto.Id" /> set.
    /// </returns>
    public Task<ProductDto> CreateProductAsync(ProductDto product);

    /// <summary>
    /// Asynchronously updates an existing product.
    /// </summary>
    /// <param name="product">The product to update in the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the product was found and updated.
    /// </returns>
    public Task<bool> UpdateProductAsync(ProductDto product);

    /// <summary>
    /// Asynchronously deletes an existing product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to delete from the database.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be a value indicating whether the product was found and deleted.
    /// </returns>
    public Task<bool> DeleteProductAsync(Guid productId);
}
