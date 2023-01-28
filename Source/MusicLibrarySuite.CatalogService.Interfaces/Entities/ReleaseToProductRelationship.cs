using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release-to-product relationship.
/// </summary>
public class ReleaseToProductRelationship
{
    /// <summary>
    /// Gets or sets the release's unique identifier.
    /// </summary>
    [Required]
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the product's unique identifier.
    /// </summary>
    [Required]
    public Guid ProductId { get; set; }

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
    /// Gets or sets the release.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public Release? Release { get; set; }

    /// <summary>
    /// Gets or sets the product.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public Product? Product { get; set; }
}
