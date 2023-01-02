using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a work-to-product relationship.
/// </summary>
public class WorkToProductRelationshipDto
{
    /// <summary>
    /// Gets or sets the work's unique identifier.
    /// </summary>
    public Guid WorkId { get; set; }

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
    /// Gets or sets the relationship's display order for the entity acting as a relationship owner (work).
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the relationship's display order used when requesting relationships for a product.
    /// </summary>
    public int ReferenceOrder { get; set; }
}
