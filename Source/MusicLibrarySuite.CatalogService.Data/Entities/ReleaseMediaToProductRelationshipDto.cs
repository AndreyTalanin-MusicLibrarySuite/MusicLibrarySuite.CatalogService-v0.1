using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for a release-media-to-product relationship.
/// </summary>
public class ReleaseMediaToProductRelationshipDto
{
    /// <summary>
    /// Gets or sets the release media's number.
    /// </summary>
    public byte MediaNumber { get; set; }

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
    /// Gets or sets the relationship's display order for the entity acting as a relationship owner (release media).
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the relationship's display order used when requesting relationships for a product.
    /// </summary>
    public int ReferenceOrder { get; set; }

    /// <summary>
    /// Gets or sets the release media.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ReleaseMediaDto? ReleaseMedia { get; set; }

    /// <summary>
    /// Gets or sets the product.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ProductDto? Product { get; set; }
}
