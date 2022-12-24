using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Controllers;

/// <summary>
/// Represents a Web API controller for the <see cref="Product" /> entity.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class ProductController : ControllerBase
{
    private readonly IProductService m_productService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductController" /> type using the specified services.
    /// </summary>
    /// <param name="productService">The product service.</param>
    public ProductController(IProductService productService)
    {
        m_productService = productService;
    }

    /// <summary>
    /// Asynchronously gets a product by its unique identifier.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the product found or <see langword="null" />.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetProductAsync([Required][FromQuery] Guid productId)
    {
        Product? product = await m_productService.GetProductAsync(productId);
        return product is not null
            ? (ActionResult<Product>)Ok(product)
            : (ActionResult<Product>)NotFound();
    }

    /// <summary>
    /// Asynchronously gets products by an array of unique identifiers.
    /// </summary>
    /// <param name="productIds">The array of unique identifiers to search for.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all found products.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Product[]>> GetProductsAsync([Required][FromQuery] Guid[] productIds)
    {
        Product[] products = await m_productService.GetProductsAsync(productIds);
        return Ok(products);
    }

    /// <summary>
    /// Asynchronously gets all products.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all products.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Product[]>> GetAllProductsAsync()
    {
        Product[] products = await m_productService.GetProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Asynchronously gets products by a product page request.
    /// </summary>
    /// <param name="pageSize">The page size.</param>
    /// <param name="pageIndex">The page index.</param>
    /// <param name="title">The filter value for the <see cref="Product.Title" /> property.</param>
    /// <param name="enabled">The filter value for the <see cref="Product.Enabled" /> property.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all products corresponding to the request configuration.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductPageResponse>> GetPagedProductsAsync([Required][FromQuery] int pageSize, [Required][FromQuery] int pageIndex, [FromQuery] string? title, [FromQuery] bool? enabled)
    {
        var productRequest = new ProductRequest()
        {
            PageSize = pageSize,
            PageIndex = pageIndex,
            Title = title,
            Enabled = enabled,
        };

        ProductPageResponse pageResponse = await m_productService.GetProductsAsync(productRequest);
        return Ok(pageResponse);
    }

    /// <summary>
    /// Asynchronously gets all product relationships by a product's unique identifier.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <param name="includeReverseRelationships">A boolean value specifying whether reverse relationships should be included.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be an array containing all product relationships.
    /// </returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductRelationship[]>> GetProductRelationshipsAsync([Required][FromQuery] Guid productId, [FromQuery] bool includeReverseRelationships)
    {
        ProductRelationship[] productRelationships = await m_productService.GetProductRelationshipsAsync(productId, includeReverseRelationships);
        return Ok(productRelationships);
    }

    /// <summary>
    /// Asynchronously creates a new product.
    /// </summary>
    /// <param name="product">The product to create.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The task's result will be the created product with properties like <see cref="Product.Id" /> set.
    /// </returns>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Product>> CreateProductAsync([Required][FromBody] Product product)
    {
        Product createdProduct = await m_productService.CreateProductAsync(product);
        return Ok(createdProduct);
    }

    /// <summary>
    /// Asynchronously updates an existing product.
    /// </summary>
    /// <param name="product">The product to update.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateProductAsync([Required][FromBody] Product product)
    {
        var result = await m_productService.UpdateProductAsync(product);
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Asynchronously deletes an existing product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to delete.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProductAsync([Required][FromQuery] Guid productId)
    {
        var result = await m_productService.DeleteProductAsync(productId);
        return result ? Ok() : NotFound();
    }
}
