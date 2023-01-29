using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a release-track-to-artist relationship where the artist has the "performer" role.
/// </summary>
public class ReleaseTrackPerformer
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
    /// Gets or sets the artist's unique identifier.
    /// </summary>
    [Required]
    public Guid ArtistId { get; set; }
}
