using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release-track-to-product relationship.
/// </summary>
public class ReleaseTrackToProductRelationship
{
    /// <summary>
    /// Gets or sets the release track's number.
    /// </summary>
    [Required]
    public byte TrackNumber { get; set; }

    /// <summary>
    /// Gets or sets the release media's number.
    /// </summary>
    [Required]
    public byte MediaNumber { get; set; }

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
    /// Gets or sets the release track.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public ReleaseTrack? ReleaseTrack { get; set; }

    /// <summary>
    /// Gets or sets the product.
    /// </summary>
    /// <remarks>This property is only used to store data returned from the database.</remarks>
    public Product? Product { get; set; }
}
