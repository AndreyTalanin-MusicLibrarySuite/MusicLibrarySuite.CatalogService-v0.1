using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services;

/// <summary>
/// Represents a product service.
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository m_productRepository;
    private readonly IMapper m_mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService" /> type using the specified services.
    /// </summary>
    /// <param name="productRepository">The product repository.</param>
    /// <param name="mapper">The AutoMapper mapper.</param>
    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        m_productRepository = productRepository;
        m_mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Product?> GetProductAsync(Guid productId)
    {
        ProductDto? productDto = await m_productRepository.GetProductAsync(productId);
        Product? product = m_mapper.Map<Product?>(productDto);
        return product;
    }

    /// <inheritdoc />
    public async Task<Product[]> GetProductsAsync()
    {
        ProductDto[] productDtoArray = await m_productRepository.GetProductsAsync();
        Product[] productArray = m_mapper.Map<Product[]>(productDtoArray);
        return productArray;
    }

    /// <inheritdoc />
    public async Task<Product[]> GetProductsAsync(IEnumerable<Guid> productIds)
    {
        ProductDto[] productDtoArray = await m_productRepository.GetProductsAsync(productIds);
        Product[] productArray = m_mapper.Map<Product[]>(productDtoArray);
        return productArray;
    }

    /// <inheritdoc />
    public async Task<ProductPageResponse> GetProductsAsync(ProductPageRequest productPageRequest)
    {
        ProductPageRequestDto productPageRequestDto = m_mapper.Map<ProductPageRequestDto>(productPageRequest);
        PageResponseDto<ProductDto> productPageResponseDto = await m_productRepository.GetProductsAsync(productPageRequestDto);
        ProductPageResponse productPageResponse = m_mapper.Map<ProductPageResponse>(productPageResponseDto);
        return productPageResponse;
    }

    /// <inheritdoc />
    public async Task<ProductRelationship[]> GetProductRelationshipsAsync(Guid productId, bool includeReverseRelationships = false)
    {
        ProductRelationshipDto[] productRelationshipDtoArray = await m_productRepository.GetProductRelationshipsAsync(productId, includeReverseRelationships);
        ProductRelationship[] productRelationshipArray = m_mapper.Map<ProductRelationship[]>(productRelationshipDtoArray);
        return productRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<Product> CreateProductAsync(Product product)
    {
        ProductDto productDto = m_mapper.Map<ProductDto>(product);
        ProductDto createdProductDto = await m_productRepository.CreateProductAsync(productDto);
        Product createdProduct = m_mapper.Map<Product>(createdProductDto);
        return createdProduct;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateProductAsync(Product product)
    {
        ProductDto productDto = m_mapper.Map<ProductDto>(product);
        var updated = await m_productRepository.UpdateProductAsync(productDto);
        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        var deleted = await m_productRepository.DeleteProductAsync(productId);
        return deleted;
    }
}
