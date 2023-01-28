using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a release-to-product relationship.
/// </summary>
public class ReleaseToProductRelationshipDto
{
    /// <summary>
    /// Gets or sets the release's unique identifier.
    /// </summary>
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the product's unique identifier.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the relationship's name.
    /// </summary>
    [StringLength(256)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the relationship's description.
    /// </summary>
    [StringLength(2048)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the relationship's display order for the entity acting as a relationship owner (release).
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the relationship's display order used when requesting relationships for a product.
    /// </summary>
    public int ReferenceOrder { get; set; }

    /// <summary>
    /// Gets or sets the release.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ReleaseDto? Release { get; set; }

    /// <summary>
    /// Gets or sets the product.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ProductDto? Product { get; set; }
}
