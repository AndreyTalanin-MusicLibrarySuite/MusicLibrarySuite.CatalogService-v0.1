using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release-to-artist relationship where the artist has the "composer" role.
/// </summary>
public class ReleaseComposer
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