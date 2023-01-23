using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release-to-artist relationship.
/// </summary>
public class ReleaseArtist
{
    /// <summary>
    /// Gets or sets the release's unique identifier.
    /// </summary>
    [Required]
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the artist's unique identifier.
    /// </summary>
    [Required]
    public Guid ArtistId { get; set; }
}
