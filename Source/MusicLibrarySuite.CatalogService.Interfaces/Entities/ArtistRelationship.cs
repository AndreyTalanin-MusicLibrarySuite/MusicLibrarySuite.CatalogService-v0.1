using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents an artist-to-artist relationship.
/// </summary>
public class ArtistRelationship
{
    /// <summary>
    /// Gets or sets the principal artist's unique identifier.
    /// </summary>
    [Required]
    public Guid ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the dependent artist's unique identifier.
    /// </summary>
    [Required]
    public Guid DependentArtistId { get; set; }

    /// <summary>
    /// Gets or sets the relationship's name.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the relationship's description.
    /// </summary>
    public string? Description { get; set; }
}
