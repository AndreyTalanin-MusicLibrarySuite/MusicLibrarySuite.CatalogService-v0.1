using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Data.Entities;

/// <summary>
/// Represents a database model and a data transfer object for an artist-to-artist relationship.
/// </summary>
public class ArtistRelationshipDto
{
    /// <summary>
    /// Gets or sets the principal artist's unique identifier.
    /// </summary>
    public Guid ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the dependent artist's unique identifier.
    /// </summary>
    public Guid DependentArtistId { get; set; }

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
    /// Gets or sets the relationship's display order.
    /// </summary>
    public int Order { get; set; }
}
