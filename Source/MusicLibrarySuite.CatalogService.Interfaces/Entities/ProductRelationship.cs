using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a product-to-product relationship.
/// </summary>
public class ProductRelationship
{
    /// <summary>
    /// Gets or sets the principal product's unique identifier.
    /// </summary>
    [Required]
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the dependent product's unique identifier.
    /// </summary>
    [Required]
    public Guid DependentProductId { get; set; }

    /// <summary>
    /// Gets or sets the relationship's name.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the relationship's description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the principal product.
    /// </summary>
    /// <remarks>This property is only used to store data returned by the application.</remarks>
    public Product? Product { get; set; }

    /// <summary>
    /// Gets or sets the dependent product.
    /// </summary>
    /// <remarks>This property is only used to store data returned by the application.</remarks>
    public Product? DependentProduct { get; set; }
}
